namespace G4ME.SourceBuilder.Types;

public static class TypeName
{
    private static readonly Dictionary<string, string> _clrTypeToCSharpAlias = new()
    {
        { "Boolean", "bool" },
        { "Byte", "byte" },
        { "SByte", "sbyte" },
        { "Char", "char" },
        { "Decimal", "decimal" },
        { "Double", "double" },
        { "Single", "float" },
        { "Int32", "int" },
        { "UInt32", "uint" },
        { "Int64", "long" },
        { "UInt64", "ulong" },
        { "Int16", "short" },
        { "UInt16", "ushort" },
        { "Object", "object" },
        { "String", "string" }
    };

    public static string ValueOf<T>()
    {
        Type type = typeof(T);

        return ValueOf(type);
    }

    public static string ValueOf(Type type)
    {
        if (_clrTypeToCSharpAlias.TryGetValue(type.Name, out var value))
        {
            return value;
        }

        return type.Name;
    }
}
