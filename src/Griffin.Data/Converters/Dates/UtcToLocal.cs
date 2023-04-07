using System;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters.Dates;

/// <summary>
///     Stores UTC in DB and local time in the class property.
/// </summary>
public class UtcToLocal : ISingleValueConverter<DateTime, DateTime>
{
    /// <inheritdoc />
    public DateTime ColumnToProperty([NotNull] DateTime utcTime)
    {
        return utcTime.ToLocalTime();
    }

    /// <inheritdoc />
    public DateTime PropertyToColumn([NotNull] DateTime localTime)
    {
        return localTime.ToUniversalTime();
    }
}
