using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Griffin.Data.Helpers;

namespace Griffin.Data.ChangeTracking.Services.Implementations;

/// <summary>
///     Implementation of <see cref="ICopyService" />.
/// </summary>
internal class EntityTraverser
{
    private readonly TraverseCallbackHandler _callback;

    public EntityTraverser(TraverseCallbackHandler callback)
    {
        _callback = callback;
    }

    /// <inheritdoc />
    public void Traverse(object source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        Traverse(null, source, 1, new List<object>());
    }

    private void Traverse(object? parent, object source, int depth, IList<object> traversedEntities)
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

        _callback(parent, source, depth);

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
                TraverseCollection(source, value, depth + 1, traversedEntities);
            }
            else
            {
                Traverse(source, value, depth + 1, traversedEntities);
            }
        }
    }

    private void TraverseCollection(object parent, object value, int depth, IList<object> traversedEntities)
    {
        bool isSimple;
        if (value.GetType().IsArray)
        {
            var elementType = value.GetType().GetElementType()!;
            isSimple = elementType.IsSimpleType();
        }
        else
        {
            var elementType = value.GetType().GetGenericArguments()[0];
            isSimple = elementType.IsSimpleType();
        }

        var list = (IEnumerable)value;
        if (isSimple)
        {
            return;
        }

        foreach (var source in list)
        {
            Traverse(parent, source, depth, traversedEntities);
        }
    }
}
