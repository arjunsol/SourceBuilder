using G4ME.SourceBuilder.Tests.Objects;
using Microsoft.CodeAnalysis.CSharp;

namespace G4ME.SourceBuilder.Tests.Unit;

public class ConstructorBuilderTests
{
    [Fact]
    public void TestConstructorInitialization_ConstructsProperly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        var constructorBuilder = new ConstructorBuilder(classBuilder);

        Assert.NotNull(constructorBuilder);
        // Additional assertions to verify initial state if necessary
    }

    [Fact]
    public void TestAddParameter_AddsParameterCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        var constructorBuilder = new ConstructorBuilder(classBuilder);
        constructorBuilder.Parameter<int>("param1");

        var constructor = constructorBuilder.Build();
        var parameter = constructor.ParameterList.Parameters.First();

        Assert.Equal("param1", parameter.Identifier.ValueText);
        Assert.NotNull(parameter.Type);
        Assert.Equal("int", parameter.Type.ToString());
    }

    [Fact]
    public void TestAddMultipleParameters_AddsAllParameters()
    {
        var classBuilder = new ClassBuilder("TestClass");
        var constructorBuilder = new ConstructorBuilder(classBuilder);
        constructorBuilder.Parameter<int>("param1").Parameter<string>("param2");

        var constructor = constructorBuilder.Build();

        Assert.Equal(2, constructor.ParameterList.Parameters.Count);
    }

    [Fact]
    public void TestAddParameterAddsNamespace_AddsNamespaceCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        var constructorBuilder = new ConstructorBuilder(classBuilder);
        constructorBuilder.Parameter<SomeClass>("param1");

        Assert.Contains(typeof(SomeClass).Namespace, classBuilder.GetRequiredNamespaces());
    }

    [Fact]
    public void TestWithBody_AddsBodyCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        var constructorBuilder = new ConstructorBuilder(classBuilder);
        constructorBuilder.Body(body => { /* body configuration */ });

        var constructor = constructorBuilder.Build();
        // TODO: Add body checks
        // Assertions for the body content, depending on how BlockBuilder is implemented
    }

    [Fact]
    public void TestBuildConstructor_BuildsConstructorCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        var constructorBuilder = new ConstructorBuilder(classBuilder);
        constructorBuilder.Parameter<int>("param1").Body(body => { /* body configuration */ });

        var constructor = constructorBuilder.Build();
        // TODO: Add body checks
        Assert.NotNull(constructor.ParameterList);
        Assert.NotNull(constructor.Body);
    }

    [Fact]
    public void ConstructorWithBaseCall_ShouldMapAllParameters()
    {
        var builder = new ConstructorBuilder(new ClassBuilder("MyClass"))
            .Parameter<int>("param1")
            .MapBase()
            .Body("int x = 1;");

        var constructorSyntax = builder.Build();

        Assert.NotNull(constructorSyntax.Initializer);
        Assert.Equal(SyntaxKind.BaseConstructorInitializer, constructorSyntax.Initializer.Kind());
        Assert.Single(constructorSyntax.Initializer.ArgumentList.Arguments);
        Assert.Equal("param1", constructorSyntax.Initializer.ArgumentList.Arguments.First().Expression.ToString());
    }

    [Fact]
    public void ConstructorWithBaseCallSpecificParams_ShouldMapSelectedParameters()
    {
        var builder = new ConstructorBuilder(new ClassBuilder("MyClass"))
            .Parameter<int>("param1")
            .Parameter<string>("param2")
            .MapBase("param1")
            .Body("int x = 1;");

        var constructorSyntax = builder.Build();

        Assert.NotNull(constructorSyntax.Initializer);
        Assert.Equal(SyntaxKind.BaseConstructorInitializer, constructorSyntax.Initializer.Kind());
        Assert.Single(constructorSyntax.Initializer.ArgumentList.Arguments);
        Assert.Equal("param1", constructorSyntax.Initializer.ArgumentList.Arguments.First().Expression.ToString());
    }
}
