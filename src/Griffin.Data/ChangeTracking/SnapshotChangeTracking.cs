using System;
using System.Threading.Tasks;
using Griffin.Data.ChangeTracking.Services;
using Griffin.Data.ChangeTracking.Services.Implementations;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data.ChangeTracking;

/// <summary>
///     Change tracker which uses snapshots to detect changes.
/// </summary>
/// <remarks>
///     <para>
///         Snapshots means that this tracker creates a copy of every fetched entity (and it's children) to be able to
///         detect changes.
///     </para>
///     <para>
///         This tracker uses the primary keys to identify entities.
///     </para>
/// </remarks>
public class SnapshotChangeTracking : IChangeTracker
{
    private readonly CopyService _copyService = new();
    private readonly IMappingRegistry _mappingRegistry;
    private readonly IEntityCache _entityCache;

    public SnapshotChangeTracking(IMappingRegistry mappingRegistry)
    {
        _mappingRegistry = mappingRegistry ?? throw new ArgumentNullException(nameof(mappingRegistry));
        _entityCache = new EntityCache(mappingRegistry);
        _copyService.Callback = (parent, current, snapshot, depth) =>
        {
            var trackedEntity = new TrackedEntity(snapshot, current, parent, depth);
            var key = _mappingRegistry.GenerateKey(trackedEntity.Current);
            _entityCache.Append(key, trackedEntity);
        };
    }

    /// <inheritdoc />
    public void Track(object item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _ = _copyService.Copy(item);
    }

    /// <inheritdoc />
    public void Refresh(object entity)
    {
        if (!_entityCache.TryFind(entity, out var cachedEntity))
        {
            throw new InvalidOperationException("Failed to find entity " + entity) { Data = { { "Entity", entity } } };
        }

        var diff = new SnapshotDiff(_mappingRegistry, _entityCache);
        var service = new CompareService(_mappingRegistry, diff);
        service.Compare(cachedEntity.Snapshot, cachedEntity.Current);
    }

    /// <inheritdoc />
    public async Task ApplyChanges(Session session)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        var diff = new SnapshotDiff(_mappingRegistry, _entityCache);
        var service = new CompareService(_mappingRegistry, diff);

        var items = _entityCache.GetDepth(1);
        foreach (var kvp in items)
        {
            service.Compare(kvp.Snapshot, kvp.Current);
        }

        //TODO: In the future, do this in bulk CRUD per entity type and depth.

        // INSERTs must be made from root objects and down so that foreign keys can be applied.
        var entitiesToInsert = _entityCache.ListForState(ChangeState.Added);
        foreach (var tracked in entitiesToInsert)
        {
            if (tracked.Parent != null)
            {
                var mapping = _mappingRegistry.Get(tracked.Parent.GetType());
                var relation = mapping.GetRelation(tracked.Current.GetType());
                if (relation != null)
                {
                    var parentId = relation.GetReferencedId(tracked.Parent);
                    if (parentId == null)
                    {
                        throw new MappingException(tracked.Parent,
                            "Failed to find referenced id for " + tracked.Current);
                    }

                    relation.SetForeignKey(tracked.Current, parentId);
                }
            }

            await session.Insert(tracked.Current);
        }

        // DELETEs must be made from leaf children and up so that foreign keys is unreferenced.
        var entitiesToRemove = _entityCache.ListForState(ChangeState.Removed);
        foreach (var entity in entitiesToRemove)
        {
            await session.Delete(entity.Current);
        }

        // UPDATEs doesn't require a specific order.
        var entitiesToUpdate = _entityCache.ListForState(ChangeState.Modified);
        foreach (var entity in entitiesToUpdate)
        {
            await session.Update(entity.Current);
        }
    }

    /// <inheritdoc />
    public ChangeState GetState(object entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (!_entityCache.TryFind(entity, out var trackedEntity))
        {
            throw new MappingException(entity,
                "Failed to find entity. If it's an added child entity, invoke 'tracker.Refresh(rootEntity);' first.");
        }

        // We have to make a copy so that GetState for un-tracked children
        // do not pollute our index (being registered as root entities).
        var indexCopy = _entityCache.Clone();

        var diff = new SnapshotDiff(_mappingRegistry, indexCopy);
        var service = new CompareService(_mappingRegistry, diff);
        service.Compare(trackedEntity.Snapshot, trackedEntity.Current);
        return trackedEntity.State;
    }
}
