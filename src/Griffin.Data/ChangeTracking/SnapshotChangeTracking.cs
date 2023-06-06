using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Data.ChangeTracking.Services;
using Griffin.Data.ChangeTracking.Services.Implementations;
using Griffin.Data.Mapper;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Mapper.Mappings.Relations;

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
    private readonly IEntityCache _entityCache;
    private readonly IMappingRegistry _mappingRegistry;

    /// <summary>
    /// </summary>
    /// <param name="mappingRegistry">Used to lookup mappings.</param>
    /// <exception cref="ArgumentNullException">arguments are not specified.</exception>
    /// <exception cref="MappingException">Failed to find mappings for entities.</exception>
    public SnapshotChangeTracking(IMappingRegistry mappingRegistry)
    {
        _mappingRegistry = mappingRegistry ?? throw new ArgumentNullException(nameof(mappingRegistry));
        _entityCache = new EntityCache(mappingRegistry);
        _copyService.Callback = (parent, current, snapshot, depth) =>
        {
            var key = _mappingRegistry.GenerateKey(current);
            if (key == null)
            {
                throw new MappingException(current, "Does not contain a value in the key property.");
            }

            var trackedEntity = new TrackedEntity(snapshot, current, parent, depth);
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
        if (!_entityCache.TryFind(entity, out var cachedEntity) || cachedEntity == null)
        {
            throw new InvalidOperationException("Failed to find entity " + entity) { Data = { { "Entity", entity } } };
        }

        var diff = new DiffToCache(_entityCache);
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

        var diff = new DiffToCache(_entityCache);
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
            if (entity.Snapshot == null)
            {
                throw new InvalidOperationException(
                    "Change tracking bug. Snapshot is empty for item marked for deletion.");
            }

            await session.Delete(entity.Snapshot);
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

        if (!_entityCache.TryFind(entity, out var trackedEntity) || trackedEntity == null)
        {
            throw new MappingException(entity,
                "Failed to find entity. If it's an added child entity, invoke 'tracker.Refresh(rootEntity);' first.");
        }

        // We have to make a copy so that GetState for un-tracked children
        // do not pollute our index (being registered as root entities).
        var indexCopy = _entityCache.Clone();

        var diff = new DiffToCache(indexCopy);
        var service = new CompareService(_mappingRegistry, diff);
        service.Compare(trackedEntity.Snapshot, trackedEntity.Current);
        return trackedEntity.State;
    }

    /// <inheritdoc />
    public async Task ApplyIsolated(Session session, object current)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (!_entityCache.TryFind(current, out var trackedEntity) || trackedEntity == null)
        {
            throw new InvalidOperationException("Fail!");
        }

        var diff = new DiffToCache(_entityCache);
        var service = new CompareService(_mappingRegistry, diff);
        service.Compare(trackedEntity.Snapshot, current);

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
        foreach (var tracked in entitiesToRemove)
        {
            await session.Delete(tracked.Current);
        }

        // UPDATEs doesn't require a specific order.
        var entitiesToUpdate = _entityCache.ListForState(ChangeState.Modified);
        foreach (var tracked in entitiesToUpdate)
        {
            await session.Update(tracked.Current);
        }
    }
}
