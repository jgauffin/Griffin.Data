using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.ChangeTracking.Services.Implementations.v2;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data.ChangeTracking.Services.Implementations;

/// <summary>
///     Used to applied changes between a snapshot entity and an un-tracked entity (which you have built yourself).
/// </summary>
/// <remarks>
///     <para>
///         Intended purpose is to be able to save entities which you have received from client side and want to persist
///         the changes to the database.
///     </para>
/// </remarks>
public class SingleEntityChangeService : IEntityCache
{
    private readonly List<TrackedEntity> _addedEntities = new();
    private readonly Dictionary<string, TrackedEntity> _existingEntities = new();
    private readonly IMappingRegistry _registry;

    /// <summary>
    /// </summary>
    /// <param name="registry">Registry with all mappings.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SingleEntityChangeService(IMappingRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public DiffReportEntity CreateReport()
    {
        var trackedItems = _existingEntities.Values.OrderBy(x => x.Depth).ToList();
        List<DiffReportEntity> items = new List<DiffReportEntity>();
        foreach (var trackedItem in trackedItems)
        {
            var item = new DiffReportEntity() { Entity = trackedItem.Current, State = trackedItem.State };
            items.Add(item);
            var parent = items.FirstOrDefault(x => x.Entity == trackedItem.Parent);
            if (parent != null)
            {
                parent.Children.Add(item);
            }
        }

        return items[0];
    }
    void IEntityCache.Insert(TrackedEntity trackedEntity)
    {
        _addedEntities.Add(trackedEntity);
    }

    void IEntityCache.Append(string key, TrackedEntity entity)
    {
        _existingEntities[key] = entity;
    }

    IEntityCache IEntityCache.Clone()
    {
        throw new NotSupportedException("Create a new instance of SingleEntityChangeService instead of cloning.");
    }

    IReadOnlyList<TrackedEntity> IEntityCache.GetDepth(int depth)
    {
        return _existingEntities.Where(x => x.Value.Depth == depth).Select(x => x.Value).ToList();
    }

    IReadOnlyList<TrackedEntity> IEntityCache.ListForState(ChangeState state)
    {
        return state == ChangeState.Added
            ? _addedEntities.OrderBy(x => x.Depth).ToList()
            : _existingEntities.Values.Where(x => x.State == state).OrderByDescending(x => x.Depth).ToList();
    }

    void IEntityCache.MarkAsModified(object entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var key = _registry.GenerateKey(entity);
        if (key == null)
        {
            throw new MappingException(entity, "Entity has no key value specified, despite being marked as modified.");
        }

        _existingEntities[key].Current = entity;
        _existingEntities[key].State = ChangeState.Modified;
    }

    void IEntityCache.MarkAsRemoved(object entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var key = _registry.GenerateKey(entity);
        if (key == null)
        {
            throw new MappingException(entity, "Entity has no key value specified, despite being marked as removed.");
        }

        _existingEntities[key].Current = entity;
        _existingEntities[key].State = ChangeState.Removed;
    }

    bool IEntityCache.TryFind(object entity, out TrackedEntity? trackedEntity)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Detect changes.
    /// </summary>
    /// <param name="session">Session</param>
    /// <param name="snapshotToCompareTo">Snapshot (not modified entity loaded in the session).</param>
    /// <param name="modifiedEntity">Entity that you have modified.</param>
    /// <returns></returns>
    public async Task PersistChanges(Session session, object snapshotToCompareTo, object modifiedEntity)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (snapshotToCompareTo == null)
        {
            throw new ArgumentNullException(nameof(snapshotToCompareTo));
        }

        if (modifiedEntity == null)
        {
            throw new ArgumentNullException(nameof(modifiedEntity));
        }

        BuildCache(snapshotToCompareTo);
        Compare(modifiedEntity);
        await ApplyChanges(session);
    }

    private async Task ApplyChanges(Session session)
    {
        var cache = (IEntityCache)this;

        // INSERTs must be made from root objects and down so that foreign keys can be applied.
        var entitiesToInsert = cache.ListForState(ChangeState.Added);
        foreach (var tracked in entitiesToInsert)
        {
            if (tracked.Parent != null)
            {
                var mapping = _registry.Get(tracked.Parent.GetType());
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
        var entitiesToRemove = cache.ListForState(ChangeState.Removed);
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
        var entitiesToUpdate = cache.ListForState(ChangeState.Modified);
        foreach (var entity in entitiesToUpdate)
        {
            await session.Update(entity.Current);
        }
    }

    private void BuildCache(object snapshot)
    {
        var copyService = new CopyService();
        var item = copyService.Copy(snapshot);


        var traverser = new EntityTraverser(VisitSnapshot);
        traverser.Traverse(snapshot);

        // Let's reset so we can detect changes for our externally loaded entity
        foreach (var entity in _existingEntities)
        {
            entity.Value.Current = null!;
        }
    }

    private void Compare(object current)
    {
        var diff = new DiffToCache(this);
        var service = new CompareService(_registry, diff);
        var items = _existingEntities.First(x => x.Value.Depth == 1);
        service.Compare(items.Value.Snapshot, current);
    }

    private void VisitCurrent(object? parent, object entity, int depth)
    {
        var mapping = _registry.Get(entity.GetType());
        var key = mapping.GenerateKey(entity);
        if (key == null)
        {
            _addedEntities.Add(new TrackedEntity(null, entity, parent, depth));
            return;
        }

        if (_existingEntities.TryGetValue(key, out var trackedEntity) && trackedEntity != null)
        {
            _existingEntities[key] = new TrackedEntity(entity, entity, parent, depth);
        }
    }

    private void VisitSnapshot(object? parent, object entity, int depth)
    {
        var mapping = _registry.Get(entity.GetType());
        var key = mapping.GenerateKey(entity);
        if (key == null)
        {
            throw new MappingException(entity, "Expected to find a key for visited snapshot.");
        }

        _existingEntities[key] = new TrackedEntity(entity, entity, parent, depth);
    }
}
