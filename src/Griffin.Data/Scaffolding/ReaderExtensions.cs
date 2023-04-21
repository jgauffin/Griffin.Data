using System;
using System.Data;

namespace Griffin.Data.Scaffolding;

/// <summary>
/// Extensions to handle DBNull.
/// </summary>
public static class ReaderExtensions
{
    /// <summary>
    /// Get a nullable integer.
    /// </summary>
    /// <param name="reader">Reader to read from.</param>
    /// <param name="name">Column name.</param>
    /// <returns>Value</returns>
    public static int? GetNullableInt(this IDataReader reader, string name)
    {
        var value = reader[name];
        if (value is DBNull)
        {
            return null;
        }

        return (int)value;
    }

    /// <summary>
    /// Get a nullable string.
    /// </summary>
    /// <param name="reader">Reader to read from.</param>
    /// <param name="name">Column name.</param>
    /// <returns>Value</returns>
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
