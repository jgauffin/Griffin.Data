using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Griffin.Data.Helpers;

/// <summary>
///     Extensions used to check built in .NET types.
/// </summary>
public static class TypeExtensions
{
    private static readonly Type[]
        CollectionTypes = { typeof(IReadOnlyList<>), typeof(IList<>), typeof(ICollection<>) };

    /// <summary>
    ///     Is it a collection/list/array?
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns><c>true</c> if it is; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Argument is null.</exception>
    public static bool IsCollection(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (type.IsArray)
        {
            return true;
        }

        if (type.IsGenericType && CollectionTypes.Contains(type.GetGenericTypeDefinition()))
        {
            return true;
        }

        if (type == typeof(IList))
        {
            return true;
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType && CollectionTypes.Contains(interfaceType.GetGenericTypeDefinition()))
            {
                return true;
            }

            if (interfaceType == typeof(IList))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Is it a built in simple (single value) type?
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns><c>true</c> if it is; otherwise <c>false</c>.</returns>
    public static bool IsSimpleType(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

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
}
