namespace Griffin.Data.Scaffolding;

internal static class StringExtensions
{
    public static string ToPascalCase(this string value)
    {
        if (value.Length < 1)
        {
            return value;
        }

        var str = char.ToUpper(value[0]);
        for (var i = 1; i < value.Length; i++)
        {
            if (value[i] == '_')
            {
                if (i + 1 < value.Length)
                {
                    str += char.ToUpper(value[i + 1]);
                    i++;
                }
            }
            else
            {
                str += value[i];
            }
        }

        return value;
    }
}
