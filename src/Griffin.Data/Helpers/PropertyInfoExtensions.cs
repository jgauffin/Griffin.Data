using System;
using System.Linq;
using System.Reflection;

namespace Griffin.Data.Helpers;

/// <summary>
///     Extensions for mapping values to the entity.
/// </summary>
public static class PropertyInfoExtensions
{
    /// <summary>
    ///     Try to create an converter
    /// </summary>
    /// <param name="columnType"></param>
    /// <param name="propertyType"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static Func<object, object>? GetColumnToPropertyConverter(Type columnType, Type propertyType)
    {
        if (propertyType.IsCollection())
            throw new NotSupportedException("Collections are not supported by the auto-converter yet.");

        var underlyingType = Nullable.GetUnderlyingType(propertyType);
        var type = underlyingType ?? propertyType;
        if (type.IsAssignableFrom(columnType)) return null;


        if (!type.IsEnum) return x => Convert.ChangeType(x, propertyType);

        return columnType == typeof(string)
            ? x => Enum.Parse(type, (string)x)
            : x => Enum.ToObject(type, (int)x);
    }

    /// <summary>
    ///     Generate a setter delegate for a property.
    /// </summary>
    /// <param name="property">Property generate for.</param>
    /// <param name="entityType">Entity that the property is declared in.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Property is null.</exception>
    /// <remarks>
    ///     <para>Tries all possible ways, including using the backing field directly.</para>
    /// </remarks>
    public static Action<object, object>? GenerateSetterDelegate(this PropertyInfo property, Type entityType)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));

        if (property.CanWrite)
            return property.SetValue;

        var setterMethod = property.GetSetMethod();
        if (setterMethod != null)
            return (entity, value) => { setterMethod.Invoke(entity, new[] { value }); };

        var camelCase = property.Name.Length == 0
            ? property.Name.ToLower()
            : char.ToLower(property.Name[0]) + property.Name[1..];

        var oldBackingFieldFormat = $"{camelCase}k__BackingField";
        var newBackingFieldFormat = $"<{property.Name}>k__BackingField";
        var customField = $"_{camelCase}";

        //property.DeclaringType.GetFields(BindingFlags.Instance|BindingFlags.NonPublic)
        var field = entityType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x =>
                x.Name == oldBackingFieldFormat || x.Name == newBackingFieldFormat ||
                x.Name.Equals(customField, StringComparison.OrdinalIgnoreCase));
        if (field == null) return null;

        return field.SetValue;
    }

    /// <summary>
    ///     Generate a setter delegate for a property.
    /// </summary>
    /// <param name="property">Property to generate for.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Property is null.</exception>
    /// <remarks>
    ///     <para>Tries all possible ways, including using the backing field directly.</para>
    /// </remarks>
    public static Action<TParent, TChild>? GenerateSetterDelegate<TParent, TChild>(this PropertyInfo property)
        where TParent : notnull where TChild : notnull
    {
        if (property == null) throw new ArgumentNullException(nameof(property));

        if (property.CanWrite)
            return (entity, value) => property.SetValue(entity, value);

        var setterMethod = property.GetSetMethod();
        if (setterMethod != null)
            return (entity, value) => { setterMethod.Invoke(entity, new object[] { value }); };

        var camelCase = property.Name.Length == 0
            ? property.Name.ToLower()
            : char.ToLower(property.Name[0]) + property.Name[1..];

        var oldBackingFieldFormat = $"{camelCase}k__BackingField";
        var newBackingFieldFormat = $"<{property.Name}>k__BackingField";
        var customField = $"_{camelCase}";

        //property.DeclaringType.GetFields(BindingFlags.Instance|BindingFlags.NonPublic)
        var field = typeof(TParent).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x =>
                x.Name == oldBackingFieldFormat || x.Name == newBackingFieldFormat ||
                x.Name.Equals(customField, StringComparison.OrdinalIgnoreCase));
        if (field == null) return null;

        return (x, y) => field.SetValue(x, y);
    }

    /// <summary>
    ///     Generate a getter delegate for a property.
    /// </summary>
    /// <param name="property">Property to generate for.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Property is null.</exception>
    /// <remarks>
    ///     <para>Tries all possible ways, including using the backing field directly.</para>
    /// </remarks>
    public static Func<TParent, TChild>? GenerateGetterDelegate<TParent, TChild>(this PropertyInfo property)
        where TParent : notnull where TChild : notnull
    {
        if (property == null) throw new ArgumentNullException(nameof(property));

        if (property.CanRead)
            return entity => (TChild)property.GetValue(entity);

        var getterMethod = property.GetGetMethod();
        if (getterMethod != null)
            return entity => (TChild)getterMethod.Invoke(entity, null);

        var camelCase = property.Name.Length == 0
            ? property.Name.ToLower()
            : char.ToLower(property.Name[0]) + property.Name[1..];

        var oldBackingFieldFormat = $"{camelCase}k__BackingField";
        var newBackingFieldFormat = $"<{property.Name}>k__BackingField";
        var customField = $"_{camelCase}";

        //property.DeclaringType.GetFields(BindingFlags.Instance|BindingFlags.NonPublic)
        var field = typeof(TParent).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x =>
                x.Name == oldBackingFieldFormat || x.Name == newBackingFieldFormat ||
                x.Name.Equals(customField, StringComparison.OrdinalIgnoreCase));
        if (field == null) return null;

        return x => (TChild)field.GetValue(x);
    }

    /// <summary>
    ///     Generate a getter delegate for a property.
    /// </summary>
    /// <param name="property">Property to generate for.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Property is null.</exception>
    /// <remarks>
    ///     <para>Tries all possible ways, including using the backing field directly.</para>
    /// </remarks>
    public static Func<object, object>? GenerateGetterDelegate(this PropertyInfo property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));

        if (property.CanRead)
            return property.GetValue;

        var getterMethod = property.GetGetMethod();
        if (getterMethod != null)
            return entity => getterMethod.Invoke(entity, null);

        var camelCase = property.Name.Length == 0
            ? property.Name.ToLower()
            : char.ToLower(property.Name[0]) + property.Name[1..];

        var oldBackingFieldFormat = $"{camelCase}k__BackingField";
        var newBackingFieldFormat = $"<{property.Name}>k__BackingField";
        var customField = $"_{camelCase}";

        //property.DeclaringType.GetFields(BindingFlags.Instance|BindingFlags.NonPublic)
        var field = property.DeclaringType!
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x =>
                x.Name == oldBackingFieldFormat || x.Name == newBackingFieldFormat ||
                x.Name.Equals(customField, StringComparison.OrdinalIgnoreCase));
        if (field == null) return null;

        return x => field.GetValue(x);
    }
}