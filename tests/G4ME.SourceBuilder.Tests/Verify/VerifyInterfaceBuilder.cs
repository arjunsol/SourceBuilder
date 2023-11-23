using G4ME.SourceBuilder.Tests.Objects;
using G4ME.SourceBuilder.Types;
using Microsoft.CodeAnalysis;

namespace G4ME.SourceBuilder.Tests;

[UsesVerify]
public class VerifyInterfaceBuilder
{
    [Fact]
    public async Task CreateBasicInterfaceTest()
    {
        var builder = new InterfaceBuilder("MyInterface");
        
        await BuildAndVerify(builder);
    }

    [Fact]
    public async Task InterfaceWithMethodsTest()
    {
        var builder = new InterfaceBuilder("MyInterface")
            .AddMethod("MethodOne", mb => { })
            .AddMethod<string>("MethodTwo", mb => mb.Parameter<string>("Thing"));

        await BuildAndVerify(builder);
    }

    [Fact]
    public async Task InterfaceExtendsTest()
    {
        var builder = new InterfaceBuilder("MyInterface")
            .Extends<ISomeInterface>();

        await BuildAndVerify(builder);
    }

    [Fact]
    public async Task InterfaceWithPropertiesTest()
    {
        var builder = new InterfaceBuilder("MyInterface")
            .Properties(p => p.Add<string>("PropertyName").Get().Set());

        await BuildAndVerify(builder);
    }

    [Fact]
    public async Task InterfaceWithAttributesTest()
    {
        var builder = new InterfaceBuilder("MyInterface")
            .WithAttributes(ab => ab.Add<SomeAttribute>());

        await BuildAndVerify(builder);
    }

    private static async Task BuildAndVerify(InterfaceBuilder builder)
    {
        var generatedInterface = builder.Build();

        await Verify(generatedInterface.NormalizeWhitespace().ToFullString());
    }
}
