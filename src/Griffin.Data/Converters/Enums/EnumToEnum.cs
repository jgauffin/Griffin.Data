namespace Griffin.Data.Converters.Enums;

internal class EnumToEnum<TEnum> : GenericToEnumConverter<TEnum, int> where TEnum : struct
{
}