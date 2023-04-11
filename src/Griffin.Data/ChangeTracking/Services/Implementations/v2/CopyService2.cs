using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

/// <summary>
///     Implementation of <see cref="ICopyService" />.
/// </summary>
public class CopyService2
{
    private IMappingRegistry _mappingRegistry;

    public CopyService2(IMappingRegistry mappingRegistry)
    {
        _mappingRegistry = mappingRegistry;
    }

    /// <inheritdoc />
    public TrackedEntity2 Copy(object source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Copy(null, source, new Dictionary<object, TrackedEntity2>());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent">Should be set from "source"</param>
    /// <param name="source"></param>
    /// <param name="depth"></param>
    /// <param name="createdInstances"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private TrackedEntity2 Copy(TrackedEntity2? parent, object source, Dictionary<object, TrackedEntity2> createdInstances)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var key = _mappingRegistry.GenerateKey(source);
        if (key == null)
        {
            throw new MappingException(source, "Failed to retrieve key value from item.");
        }

        var copy = Activator.CreateInstance(source.GetType(), true)!;
        var tracked = new TrackedEntity2(key, copy, 1);
        createdInstances[source] = tracked;
        parent?.AddChild(tracked);

        var fields = source.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType.IsSimpleType())
            {
                field.SetValue(copy, field.GetValue(source));
                continue;
            }

            var value = field.GetValue(source);
            if (value == null)
            {
                continue;
            }

            if (createdInstances.TryGetValue(value, out var existingCopy))
            {
                field.SetValue(copy, existingCopy);
            }

            var propertyCopy = field.FieldType.IsCollection()
                ? CopyCollection(tracked, value, createdInstances)
                : Copy(tracked, value, createdInstances);

            field.SetValue(copy, propertyCopy);
        }

        return tracked;
    }

    private object CopyCollection(TrackedEntity2 parent, object value, Dictionary<object, TrackedEntity2> createdInstances)
    {
        object listCopy;
        Action<object> addMethod;
        bool isSimple;
        if (value.GetType().IsArray)
        {
            var elementType = value.GetType().GetElementType()!;
            isSimple = elementType.IsSimpleType();
            var length = (int)value.GetType().GetProperty("Length")!.GetValue(value)!;
            var a = Array.CreateInstance(elementType, length);
            var index = 0;
            addMethod = item => a.SetValue(item, index++);
            listCopy = a;
        }
        else
        {
            var elementType = value.GetType().GetGenericArguments()[0];
            isSimple = elementType.IsSimpleType();
            var l = (IList)Activator.CreateInstance(value.GetType())!;
            addMethod = item => l.Add(item);
            listCopy = l;
        }

        var list = (IEnumerable)value;
        if (isSimple)
        {
            foreach (var source in list)
            {
                addMethod(source);
            }
        }
        else
        {
            foreach (var source in list)
            {
                if (createdInstances.TryGetValue(source, out var copy))
                {
                    addMethod(copy);
                }
                else
                {
                    var elementCopy = Copy(parent, source, createdInstances);
                    addMethod(elementCopy);
                }
            }
        }

        return listCopy;
    }
}
