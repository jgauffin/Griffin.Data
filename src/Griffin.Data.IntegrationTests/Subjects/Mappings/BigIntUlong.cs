using Griffin.Data.Converters;

namespace Griffin.Data.Tests.Entities.Mappings;

internal class BigIntUlong : ISingleValueConverter<long, ulong>
{
    public static readonly BigIntUlong Instance = new();

    public ulong ColumnToProperty(long value)
    {
        return (ulong)value;
    }

    public long PropertyToColumn(ulong value)
    {
        return (long)value;
    }
}
