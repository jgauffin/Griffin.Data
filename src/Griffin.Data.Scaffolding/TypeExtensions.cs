namespace Griffin.Data.Scaffolding;

internal static class TypeExtensions
{
    private static readonly Dictionary<Type, string> Aliases =
        new()
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(void), "void" }
        };

    public static string AsAlias(this Type type)
    {
        return Aliases.TryGetValue(type, out var a) ? a : type.Name;
    }
}
