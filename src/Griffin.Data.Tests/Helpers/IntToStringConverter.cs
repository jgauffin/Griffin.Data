using Griffin.Data.Converters;

namespace Griffin.Data.Tests.Helpers;

public class IntToStringConverter : ISingleValueConverter<int, string>
{
    public string ColumnToProperty(int value)
    {
        return value.ToString();
    }

    public int PropertyToColumn(string value)
    {
        return int.Parse(value);
    }
}
