using System;
using System.Linq;
using System.Reflection;

namespace Griffin.Data.Helpers;

public static class PropertyInfoExtensions
{
    public static object ConvertFromColumnToPropertyValue(this object value, Type propertyType)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        if (propertyType.IsCollection()) return value;

        // Many columns are still long instead of int, which is enough.
        if (value is long l && propertyType == typeof(int)) return (int)l;

        if (propertyType.IsInstanceOfType(value)) return value;

        var underlyingType = Nullable.GetUnderlyingType(propertyType);
        var type = underlyingType ?? propertyType;

        if (!type.IsEnum) return Convert.ChangeType(value, type);

        return value is string str
            ? Enum.Parse(type, str)
            : Enum.ToObject(type, value);
    }

    public static Func<object, object?>? GenerateGetterDelegate(this PropertyInfo property)
    {
        if (!property.CanRead) return null;

        return property.GetValue;
    }

    public static Action<object, object>? GenerateSetterDelegate(this PropertyInfo property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));

        if (property == null) throw new ArgumentNullException(nameof(property));

        if (property.CanWrite)
            return (entity, value) =>
            {
                value = value.ConvertFromColumnToPropertyValue(property.PropertyType);
                property.SetValue(entity, value);
            };

        var setterMethod = property.GetSetMethod();
        if (setterMethod != null)
            return (entity, value) =>
            {
                value = value.ConvertFromColumnToPropertyValue(property.PropertyType);
                setterMethod.Invoke(entity, new[] { value });
            };

        var camelCase = property.Name.Length == 0
            ? property.Name.ToLower()
            : char.ToLower(property.Name[0]) + property.Name[1..];

        var oldBackingFieldFormat = $"{camelCase}k__BackingField";
        var newBackingFieldFormat = $"<{property.Name}>k__BackingField";
        var customField = $"_{camelCase}";

        var declaringType = property.DeclaringType;
        if (declaringType == null)
            throw new InvalidOperationException($"DeclaringType is not specified for '{property.Name}'.");

        //property.DeclaringType.GetFields(BindingFlags.Instance|BindingFlags.NonPublic)
        var field = declaringType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x =>
                x.Name == oldBackingFieldFormat || x.Name == newBackingFieldFormat ||
                x.Name.Equals(customField, StringComparison.OrdinalIgnoreCase));
        if (field == null) return null;

        return (entity, value) =>
        {
            value = value.ConvertFromColumnToPropertyValue(property.PropertyType);
            field.SetValue(entity, value);
        };
    }
}