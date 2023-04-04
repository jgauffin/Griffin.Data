using System;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters.Enums;

/// <summary>
///     Store an enum as a string in the database.
/// </summary>
/// <typeparam name="TEnum">Enum to handle.</typeparam>
public class StringToEnum<TEnum> : ISingleValueConverter<string, TEnum> where TEnum : struct
{
    /// <inheritdoc />
    public TEnum ColumnToProperty([NotNull] string value)
    {
        if (!Enum.TryParse<TEnum>(value, true, out var enumValue))
            throw new InvalidOperationException("Failed to convert '" + value + "' to enum " + typeof(TEnum));

        return enumValue;
    }

    /// <inheritdoc />
    public string PropertyToColumn([NotNull] TEnum value)
    {
        return value.ToString();
    }
}