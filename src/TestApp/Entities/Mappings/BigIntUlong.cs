using Griffin.Data.Converters;

namespace TestApp.Entities.Mappings;

internal class BigIntUlong : ISingleValueConverter<long, ulong>
{
    public static readonly BigIntUlong Instance = new();

    /// <inheritdoc />
    public ulong ColumnToProperty(long value)
    {
        return (ulong)value;
    }

    /// <inheritdoc />
    public long PropertyToColumn(ulong value)
    {
        return (long)value;
    }
}