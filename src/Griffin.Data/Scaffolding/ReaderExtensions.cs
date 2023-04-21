using System;
using System.Data;

namespace Griffin.Data.Scaffolding;

public static class ReaderExtensions
{
    public static int? GetNullableInt(this IDataReader reader, string name)
    {
        var value = reader[name];
        if (value is DBNull)
        {
            return null;
        }

        return (int)value;
    }

    public static string? GetNullableString(this IDataReader reader, string name)
    {
        var value = reader[name];
        if (value is DBNull)
        {
            return null;
        }

        return (string)value;
    }
}
