using System;
using System.Data;

namespace Griffin.Data.Scaffolding.Helpers;

public static class DataRecordExtensions
{
    public static int? GetNullableInt(this IDataRecord record, int columnIndex)
    {
        var value = record.GetValue(columnIndex);
        if (value is DBNull)
        {
            return null;
        }

        return (int?)value;
    }

    public static string? GetNullableString(this IDataRecord record, int columnIndex)
    {
        var value = record.GetValue(columnIndex);
        if (value is DBNull)
        {
            return null;
        }

        return (string)value;
    }
}
