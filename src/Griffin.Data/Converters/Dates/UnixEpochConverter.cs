using System;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters.Dates;

/// <summary>
///     Epoch time in milliseconds.
/// </summary>
public class DateTimeToUnixTimeMs : ISingleValueConverter<DateTime, long>
{
    private static readonly DateTime Epoch = new(1970, 1, 1);

    public long ColumnToProperty([NotNull] DateTime value)
    {
        return (long)value.Subtract(Epoch).TotalMilliseconds;
    }

    public DateTime PropertyToColumn([NotNull] long value)
    {
        return Epoch.AddMilliseconds(value);
    }

    public long Convert(DateTime value)
    {
        return (long)value.Subtract(Epoch).TotalMilliseconds;
    }
}