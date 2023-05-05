using System.Collections.Generic;
using System.Linq;
using Griffin.Data.Mapper;
using Griffin.Data.Mapper.Mappings;

namespace Griffin.Data.ChangeTracking.Services;

internal class EntityCache : IEntityCache
{
    private readonly IMappingRegistry _mappingRegistry;
    private List<TrackedEntity> _addedEntities = new();
    private Dictionary<string, TrackedEntity> _existingEntities = new();

    public EntityCache(IMappingRegistry mappingRegistry)
    {
        _mappingRegistry = mappingRegistry;
    }

    public bool TryFind(object entity, out TrackedEntity? trackedEntity)
    {
        var mapping = _mappingRegistry.Get(entity.GetType());
        var key = mapping.GenerateKey(entity);
        if (key == null)
        {
            trackedEntity = _addedEntities.FirstOrDefault(x => x.Current == entity);
            return trackedEntity != null;
        }

        return _existingEntities.TryGetValue(key, out trackedEntity);
    }

    public void MarkAsModified(object entity)
    {
        var e = GetByKey(entity);
        e.State = ChangeState.Modified;
    }

    public void MarkAsRemoved(object entity)
    {
        var e = GetByKey(entity);
        e.State = ChangeState.Removed;
    }

    public IReadOnlyList<TrackedEntity> GetDepth(int depth)
    {
        var values = _existingEntities.Values
            .Where(x => x.Depth == depth)
            .Select(x => x)
            .Union(_addedEntities.Where(x => x.Depth == depth));

        return values.ToList();
    }

    public void Insert(TrackedEntity trackedEntity)
    {
        _addedEntities.Add(trackedEntity);
    }

    public void Append(string key, TrackedEntity entity)
    {
        _existingEntities[key] = entity;
    }

    public IEntityCache Clone()
    {
        return new EntityCache(_mappingRegistry)
        {
            _addedEntities = _addedEntities.ToList(),
            _existingEntities = new Dictionary<string, TrackedEntity>(_existingEntities)
        };
    }

    public IReadOnlyList<TrackedEntity> ListForState(ChangeState state)
    {
        if (state == ChangeState.Added)
        {
            return _addedEntities.OrderBy(x => x.Depth).ToList();
        }

        return _existingEntities.Values.Where(x => x.State == state).OrderByDescending(x => x.Depth).ToList();
    }

    public void Add(TrackedEntity trackedEntity)
    {
        _addedEntities.Add(trackedEntity);
    }

    private TrackedEntity GetByKey(object entity)
    {
        var mapping = _mappingRegistry.Get(entity.GetType());
        var key = mapping.GenerateKey(entity);
        if (key == null)
        {
            throw new MappingException(entity, "A new entity was marked as modified instead of added.");
        }

        if (!_existingEntities.TryGetValue(key, out var e))
        {
            throw new MappingException(entity, "Failed to find entity in the cache.");
        }

        return e;
    }
}
