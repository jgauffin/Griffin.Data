using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper.Mappings;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

/// <summary>
///     Generates a list of entities with meta data.
/// </summary>
internal class EntityTraverser2
{
    private readonly IMappingRegistry _mappingRegistry;
    private List<TrackedEntity2> _trackedEntities = new();

    public EntityTraverser2(IMappingRegistry mappingRegistry)
    {
        _mappingRegistry = mappingRegistry ?? throw new ArgumentNullException(nameof(mappingRegistry));
    }

    /// <summary>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IReadOnlyList<TrackedEntity2> Traverse(object source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        _trackedEntities = new List<TrackedEntity2>();

        var key = _mappingRegistry.GenerateKey(source);
        var root = new TrackedEntity2(key, source, 0);
        Traverse(root, source, new List<object>());
        return _trackedEntities;
    }

    private void Traverse(TrackedEntity2 parent, object source, IList<object> traversedEntities)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (traversedEntities.Contains(source))
        {
            return;
        }

        traversedEntities.Add(source);

        var key = _mappingRegistry.GenerateKey(source);
        var entity = new TrackedEntity2(key, source, parent.Depth + 1);
        parent.AddChild(entity);
        _trackedEntities.Add(entity);

        var fields = source.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType.IsSimpleType())
            {
                continue;
            }

            var value = field.GetValue(source);
            if (value == null)
            {
                continue;
            }

            if (traversedEntities.Contains(value))
            {
                continue;
            }

            if (field.FieldType.IsCollection())
            {
                TraverseCollection(entity, value, traversedEntities);
            }
            else
            {
                Traverse(entity, value, traversedEntities);
            }
        }
    }

    private void TraverseCollection(TrackedEntity2 parent, object collection, IList<object> traversedEntities)
    {
        bool isSimple;
        if (collection.GetType().IsArray)
        {
            var elementType = collection.GetType().GetElementType()!;
            isSimple = elementType.IsSimpleType();
        }
        else
        {
            var elementType = collection.GetType().GetGenericArguments()[0];
            isSimple = elementType.IsSimpleType();
        }

        var list = (IEnumerable)collection;
        if (isSimple)
        {
            return;
        }

        foreach (var source in list)
        {
            Traverse(parent, source, traversedEntities);
        }
    }
}
