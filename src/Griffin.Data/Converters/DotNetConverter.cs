using System;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters;

internal class DotNetConverter<TColumn, TProperty> : ISingleValueConverter<TColumn, TProperty>
{
    [return: NotNull]
    public TProperty ColumnToProperty([DisallowNull] [NotNull] TColumn value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return (TProperty)Convert.ChangeType(value, typeof(TProperty));
    }

    [return: NotNull]
    public TColumn PropertyToColumn([DisallowNull] [NotNull] TProperty value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return (TColumn)Convert.ChangeType(value, typeof(TColumn));
    }
}