using System;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters.Enums;

/// <summary>
///     Store an enum as an int in the database.
/// </summary>
/// <typeparam name="TEnum">Type of enum to handle.</typeparam>
public class IntToEnum<TEnum> : ISingleValueConverter<int, TEnum> where TEnum:notnull
{
    private readonly GenericToEnumConverter<int, TEnum> _converter = new();

    /// <inheritdoc />
    [return: NotNull]
    public TEnum ColumnToProperty([NotNull] int value)
    {
        return _converter.ColumnToProperty(value);
    }

    /// <inheritdoc />
    [return: NotNull]
    public int PropertyToColumn([DisallowNull] [NotNull] TEnum value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return _converter.PropertyToColumn(value);
    }
}