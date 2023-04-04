using System;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters.Dates;

/// <summary>
///     Lets you store unix epoch (milliseconds) in the database while using a <c>DateTime</c> property in your classes.
/// </summary>
public class UnixEpochConverter : ISingleValueConverter<long, DateTime>
{
    private static readonly DateTime Epoch = new(1970, 1, 1);

    /// <inheritdoc />
    public long PropertyToColumn([NotNull] DateTime value)
    {
        return (long)value.Subtract(Epoch).TotalMilliseconds;
    }

    /// <inheritdoc />
    public DateTime ColumnToProperty([NotNull] long value)
    {
        return Epoch.AddMilliseconds(value);
    }
}