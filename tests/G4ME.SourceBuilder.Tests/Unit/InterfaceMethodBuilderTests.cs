using G4ME.SourceBuilder.Tests.Objects;
using G4ME.SourceBuilder.Types;

namespace G4ME.SourceBuilder.Tests.Unit;

public class InterfaceMethodBuilderTests
{
    [Fact]
    public void DefaultMethodCreation_ShouldCreateMethod()
    {
        var parentBuilder = new InterfaceBuilder("MyInterface");
        var methodBuilder = new InterfaceMethodBuilder(parentBuilder, "MyMethod");

        var method = methodBuilder.Build();

        Assert.NotNull(method);
        Assert.Equal("MyMethod", method.Identifier.ValueText);
        Assert.Equal("void", method.ReturnType.ToString());
    }

    [Fact]
    public void AddParameter_ShouldAddParameterToMethod()
    {
        var parentBuilder = new InterfaceBuilder("MyInterface");
        var methodBuilder = new InterfaceMethodBuilder(parentBuilder, "MyMethod")
            .Parameter<string>("param");

        var method = methodBuilder.Build();

        Assert.Single(method.ParameterList.Parameters);
        var parameter = method.ParameterList.Parameters[0];
        Assert.NotNull(parameter.Type);
        Assert.Equal("param", parameter.Identifier.ValueText);
        Assert.Equal("string", parameter.Type.ToString());
    }

    [Fact]
    public void AddAttributes_ShouldAddAttributesToMethod()
    {
        var parentBuilder = new InterfaceBuilder("MyInterface");
        var methodBuilder = new InterfaceMethodBuilder(parentBuilder, "MyMethod")
            .Attributes(ab => ab.Add<SomeAttribute>());

        var method = methodBuilder.Build();

        Assert.Single(method.AttributeLists);
        Assert.Single(method.AttributeLists[0].Attributes);
        Assert.Equal("Some", method.AttributeLists[0].Attributes[0].Name.ToString());
    }

    [Fact]
    public void Build_ShouldReturnCorrectMethodSyntax()
    {
        var parentBuilder = new InterfaceBuilder("MyInterface");
        var methodBuilder = new InterfaceMethodBuilder(parentBuilder, "MyMethod")
            .Parameter<int>("number")
            .Attributes(ab => ab.Add<SomeAttribute>());

        var method = methodBuilder.Build();

        Assert.Equal("MyMethod", method.Identifier.ValueText);
        Assert.Single(method.ParameterList.Parameters);
        var parameter = method.ParameterList.Parameters[0];
        Assert.Equal("number", parameter.Identifier.ValueText);
        Assert.NotNull(parameter.Type);
        Assert.Equal("int", parameter.Type.ToString());
        Assert.Single(method.AttributeLists);
        Assert.Single(method.AttributeLists[0].Attributes);
        Assert.Equal("Some", method.AttributeLists[0].Attributes[0].Name.ToString());
    }
}
