using System;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters.Enums;

/// <summary>
///     Store an enum as a byte in the database.
/// </summary>
/// <typeparam name="TEnum">Type of enum to handle.</typeparam>
public class ByteToEnum<TEnum> : ISingleValueConverter<byte, TEnum> where TEnum:notnull
{
    private readonly GenericToEnumConverter<byte, TEnum> _converter = new();

    /// <inheritdoc />
    [return: NotNull]
    public TEnum ColumnToProperty([NotNull] byte value)
    {
        return _converter.ColumnToProperty(value);
    }

    /// <inheritdoc />
    [return: NotNull]
    public byte PropertyToColumn([DisallowNull] [NotNull] TEnum value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return _converter.PropertyToColumn(value);
    }
}