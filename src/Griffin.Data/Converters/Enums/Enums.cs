namespace Griffin.Data.Converters.Enums;

public static class Enums
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static ByteToEnum<TEnum> EnumConverter<TEnum>() where TEnum : struct
    {
        return new ByteToEnum<TEnum>();
    }
}