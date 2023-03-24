using System;

namespace Griffin.Data.Converters.Enums;

public class StringToEnum<TEnum> : ISingleValueConverter<string, TEnum> where TEnum : struct
{
    public TEnum ColumnToProperty(string value)
    {
        if (!Enum.TryParse<TEnum>(value, true, out var enumValue))
            throw new InvalidOperationException("Failed to convert '" + value + "' to enum " + typeof(TEnum));

        return enumValue;
    }

    public string PropertyToColumn(TEnum value)
    {
        return value.ToString();
    }
}