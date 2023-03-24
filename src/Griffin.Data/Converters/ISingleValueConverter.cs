using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters;

public interface ISingleValueConverter<TColumn, TProperty>
{
    [return: NotNull]
    TProperty ColumnToProperty([NotNull] TColumn value);

    [return: NotNull]
    TColumn PropertyToColumn([NotNull] TProperty value);
}