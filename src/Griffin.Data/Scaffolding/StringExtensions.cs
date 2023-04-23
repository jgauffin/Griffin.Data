using System;
using Griffin.Data.Helpers;

namespace Griffin.Data.Scaffolding;

/// <summary>
///     Extension used to transform names into .NET friendly names.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Create pascal case from underscore casing (or name with spaces).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToPascalCase(this string value)
    {
        if (value.Length < 1)
        {
            return value;
        }

        var str = "" + char.ToUpper(value[0]);
        for (var i = 1; i < value.Length; i++)
        {
            if (value[i] == '_' || value[i] == ' ')
            {
                if (i + 1 < value.Length)
                {
                    str += char.ToUpper(value[i + 1]);
                    i++;
                }
            }
            else
            {
                str += value[i];
            }
        }

        return str;
    }

    /// <summary>
    ///     Clean a column name (
    /// </summary>
    /// <param name="columnName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ToPropertyName(this string columnName)
    {
        if (columnName == null)
        {
            throw new ArgumentNullException(nameof(columnName));
        }

        return columnName.ToPascalCase().Singularize();
    }
}
