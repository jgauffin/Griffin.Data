using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Griffin.Data;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

/// <summary>
///     Implementation of <see cref="ICopyService" />.
/// </summary>
internal class EntityTraverser2
{
    private readonly IMappingRegistry _mappingRegistry;
    private readonly List<TrackedEntity2> _trackedEntities = new();

    public EntityTraverser2(IMappingRegistry mappingRegistry)
    {
        _mappingRegistry = mappingRegistry;
    }

    /// <inheritdoc />
    public IReadOnlyList<TrackedEntity2> Traverse(object source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        _trackedEntities.Clear();

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
                TraverseCollection(parent, value, traversedEntities);
            }
            else
            {
                Traverse(parent, value, traversedEntities);
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
