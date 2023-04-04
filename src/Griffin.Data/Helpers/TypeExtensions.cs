using System;
using System.Collections;
using System.Collections.Generic;

namespace Griffin.Data.Helpers;

/// <summary>
/// Extensions used to check built in .NET types.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Is it a built in simple (single value) type?
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns><c>true</c> if it is; otherwise <c>false</c>.</returns>
    public static bool IsSimpleType(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        var typeToCheck = Nullable.GetUnderlyingType(type) ?? type;

        return typeToCheck.IsPrimitive
               || typeToCheck.IsEnum
               || typeToCheck == typeof(string)
               || typeToCheck == typeof(decimal)
               || typeToCheck == typeof(DateTime)
               || typeToCheck == typeof(DateTimeOffset)
               || typeToCheck == typeof(TimeSpan)
               || typeToCheck == typeof(Guid)
               || typeToCheck == typeof(Uri);
    }

    /// <summary>
    /// Is it a collection/list/array?
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns><c>true</c> if it is; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Argument is null.</exception>
    public static bool IsCollection(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (type.IsArray)
            return true;

        foreach (var i in type.GetInterfaces())
        {
            switch (i.IsGenericType)
            {
                case true when i.GetGenericTypeDefinition() == typeof(IList<>):
                case true when i.GetGenericTypeDefinition() == typeof(ICollection<>):
                    return true;
            }

            if (i == typeof(IList))
                return true;
        }

        return false;
    }
}