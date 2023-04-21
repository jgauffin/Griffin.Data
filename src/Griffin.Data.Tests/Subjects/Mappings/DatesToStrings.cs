using System.Globalization;
using Griffin.Data.Converters;

namespace Griffin.Data.Tests.Subjects.Mappings;

public class DatesToStrings : ISingleValueConverter<string, DateTime>
{
    public DateTime ColumnToProperty(string value)
    {
        return DateTime.Parse(value, CultureInfo.InvariantCulture);
    }

    public string PropertyToColumn(DateTime value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}
