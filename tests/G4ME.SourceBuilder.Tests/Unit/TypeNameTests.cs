namespace G4ME.SourceBuilder.Tests.Unit;

public class TypeNameTests
{
    [Theory]
    [InlineData(typeof(bool), "bool")]
    [InlineData(typeof(byte), "byte")]
    [InlineData(typeof(sbyte), "sbyte")]
    [InlineData(typeof(char), "char")]
    [InlineData(typeof(decimal), "decimal")]
    [InlineData(typeof(double), "double")]
    [InlineData(typeof(float), "float")]
    [InlineData(typeof(int), "int")]
    [InlineData(typeof(uint), "uint")]
    [InlineData(typeof(long), "long")]
    [InlineData(typeof(ulong), "ulong")]
    [InlineData(typeof(short), "short")]
    [InlineData(typeof(ushort), "ushort")]
    [InlineData(typeof(object), "object")]
    [InlineData(typeof(string), "string")]
    [InlineData(typeof(MyCustomClass), nameof(MyCustomClass))]
    [InlineData(typeof(MyCustomStruct), nameof(MyCustomStruct))]
    public void ValueOf_ReturnsCorrectTypeName(Type type, string expectedTypeName)
    {
        string typeName = TypeName.ValueOf(type);
        
        Assert.Equal(expectedTypeName, typeName);
    }

    [Fact]
    public void ValueOf_GenericPrimitive_ReturnsCorrectTypeName()
    {
        string expected = "string";

        string typeName = TypeName.ValueOf<string>();

        Assert.Equal(expected, typeName);
    }

    [Fact]
    public void ValueOf_GenericType_ReturnsCorrectTypeName()
    {
        string expected = "MyCustomClass";

        string typeName = TypeName.ValueOf<MyCustomClass>();

        Assert.Equal(expected, typeName);
    }

    class MyCustomClass { }
    struct MyCustomStruct { }
}
