using System.Collections;
using System.Reflection;
using Griffin.Data.Helpers;

namespace Griffin.Data.ChangeTracking.Helpers;

internal class CopyService
{
    public object Copy(object source, Dictionary<object, object> createdInstances)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var copy = Activator.CreateInstance(source.GetType(), true)!;
        createdInstances[source] = copy;

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
            if (value == null) continue;

            if (field.FieldType.IsCollection()) value = CopyCollection(value, createdInstances);

            if (createdInstances.TryGetValue(value, out var existingCopy)) field.SetValue(copy, existingCopy);

            value = Copy(value, createdInstances);
            field.SetValue(copy, value);
        }

        return copy;
    }

    private object CopyCollection(object value, Dictionary<object, object> createdInstances)
    {
        object listCopy;
        Action<object> addMethod;
        bool isSimple;
        if (value.GetType().IsArray)
        {
            var elementType = value.GetType().GetElementType()!;
            isSimple = elementType.IsSimpleType();
            var length = (int)value.GetType().GetProperty("Count")!.GetValue(value)!;
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
            foreach (var source in list)
                addMethod(source);
        else
            foreach (var source in list)
                if (createdInstances.TryGetValue(source, out var copy))
                {
                    addMethod(copy);
                }
                else
                {
                    var elementCopy = Copy(source, createdInstances);
                    addMethod(elementCopy);
                }

        return listCopy;
    }
}