using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Griffin.Data.Helpers;

namespace Griffin.Data.ChangeTracking.Services.Implementations;

/// <summary>
///     Implementation of <see cref="ICopyService" />.
/// </summary>
public class CopyService : ICopyService
{
    /// <summary>
    ///     Callback invoked each time an entity have been copied.
    /// </summary>
    public CopyCallbackHandler? Callback { get; set; }

    /// <inheritdoc />
    public object Copy(object source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Copy(null, source, 1, new Dictionary<object, object>());
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
    private object Copy(object? parent, object source, int depth, Dictionary<object, object> createdInstances)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var copy = Activator.CreateInstance(source.GetType(), true)!;
        createdInstances[source] = copy;
        Callback?.Invoke(parent, source, copy, depth);

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
                ? CopyCollection(source, value, depth + 1, createdInstances)
                : Copy(source, value, depth + 1, createdInstances);

            field.SetValue(copy, propertyCopy);
        }

        return copy;
    }

    private object CopyCollection(object parent, object value, int depth, Dictionary<object, object> createdInstances)
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
                    var elementCopy = Copy(parent, source, depth + 1, createdInstances);
                    addMethod(elementCopy);
                }
            }
        }

        return listCopy;
    }
}
