using System;
using System.Collections;
using System.Collections.Generic;

namespace Griffin.Data.Helpers;

internal static class TypeExtensions
{
    public static bool IsSimpleType(this Type type)
    {
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

    public static bool IsCollection(this Type type)
    {
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