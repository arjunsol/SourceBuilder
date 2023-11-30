using G4ME.SourceBuilder.Tests.Objects;
using G4ME.SourceBuilder.Types;
using Microsoft.CodeAnalysis;

namespace G4ME.SourceBuilder.Tests.Verified;

[UsesVerify]
public class VerifyInterfaceBuilder
{
    [Fact]
    public async Task CreateBasicInterfaceTest()
    {
        IInterfaceBuilder builder = new InterfaceBuilder("MyInterface");
        
        await BuildAndVerify(builder);
    }

    [Fact]
    public async Task InterfaceWithMethodsTest()
    {
        IInterfaceBuilder builder = new InterfaceBuilder("MyInterface");

        builder.AddMethod("MethodOne", mb => { })
               .AddMethod<string>("MethodTwo", mb => mb.Parameter<string>("Thing"));

        await BuildAndVerify(builder);
    }

    [Fact]
    public async Task InterfaceExtendsTest()
    {
        IInterfaceBuilder builder = new InterfaceBuilder("MyInterface");
        
        builder.Extends<ISomeInterface>();

        await BuildAndVerify(builder);
    }

    [Fact]
    public async Task InterfaceWithPropertiesTest()
    {
        IInterfaceBuilder builder = new InterfaceBuilder("MyInterface");

        builder.Properties(p => p.Add<string>("PropertyName").Get().Set());

        await BuildAndVerify(builder);
    }

    [Fact]
    public async Task InterfaceWithAttributesTest()
    {
        IInterfaceBuilder builder = new InterfaceBuilder("MyInterface");

        builder.Attributes(ab => ab.Add<SomeAttribute>())
               .AddMethod("Thing", mb => mb
                   .Attributes(a => a
                       .Add<SomeAttribute>()));

        await BuildAndVerify(builder);
    }

    private static async Task BuildAndVerify(ITypeBuilder<InterfaceDeclarationSyntax> builder)
    {
        var generatedInterface = builder.Build();

        await Verify(generatedInterface.NormalizeWhitespace().ToFullString());
    }
}
