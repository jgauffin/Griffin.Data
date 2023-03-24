using System;

namespace Griffin.Data.Converters.Dates;

/// <summary>
///     Stores UTC in DB and local time in properties.
/// </summary>
public class UtcToLocal : ISingleValueConverter<DateTime, DateTime>
{
    public DateTime ColumnToProperty(DateTime utcTime)
    {
        return utcTime.ToLocalTime();
    }

    public DateTime PropertyToColumn(DateTime localTime)
    {
        return localTime.ToUniversalTime();
    }
}