using System;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters;

/// <summary>
///     Converter that uses te built in <see cref="Convert" /> class.
/// </summary>
/// <typeparam name="TColumn">Type of table column.</typeparam>
/// <typeparam name="TProperty">Type of class property.</typeparam>
public class DotNetConverter<TColumn, TProperty> : ISingleValueConverter<TColumn, TProperty>
    where TColumn : notnull where TProperty : notnull
{
    /// <inheritdoc />
    [return: NotNull]
    public TProperty ColumnToProperty([DisallowNull] [NotNull] TColumn value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return (TProperty)Convert.ChangeType(value, typeof(TProperty));
    }

    /// <inheritdoc />
    [return: NotNull]
    public TColumn PropertyToColumn([DisallowNull] [NotNull] TProperty value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return (TColumn)Convert.ChangeType(value, typeof(TColumn));
    }
}
