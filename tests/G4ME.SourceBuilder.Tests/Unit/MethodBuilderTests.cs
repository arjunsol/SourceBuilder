using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Text.Json.Serialization;
using G4ME.SourceBuilder.Tests.Objects;
using Moq;
using Xunit;

namespace G4ME.SourceBuilder.Tests.Unit;

public class MethodBuilderTests
{
    private readonly Mock<IClassBuilder> _mockClassBuilder;

    public MethodBuilderTests()
    {
        _mockClassBuilder = new Mock<IClassBuilder>();
    }

    [Fact]
    public void TestConstructorInitialization_ConstructsProperly()
    {
        var methodBuilder = new MethodBuilder(_mockClassBuilder.Object, "TestMethod");
        var method = methodBuilder.Build();

        Assert.NotNull(method);
        Assert.Equal("TestMethod", method.Identifier.ValueText);
        Assert.Contains(method.Modifiers, m => m.IsKind(SyntaxKind.PublicKeyword));
    }

    [Fact]
    public void TestAddParameter_AddsParameterCorrectly()
    {
        var methodBuilder = new MethodBuilder(_mockClassBuilder.Object, "void", "TestMethod");
        methodBuilder.Parameter<int>("param1");

        var method = methodBuilder.Build();
        var parameter = method.ParameterList.Parameters.First();

        Assert.Equal("param1", parameter.Identifier.ValueText);
        Assert.NotNull(parameter.Type);
        Assert.Equal("int", parameter.Type.ToString());
    }

    [Fact]
    public void TestAddMultipleParameters_AddsAllParameters()
    {
        var methodBuilder = new MethodBuilder(_mockClassBuilder.Object, "void", "TestMethod");
        methodBuilder.Parameter<int>("param1").Parameter<string>("param2");

        var method = methodBuilder.Build();

        Assert.Equal(2, method.ParameterList.Parameters.Count);
    }

    [Fact]
    public void TestAddParameterAddsNamespace_AddsNamespaceCorrectly()
    {
        var methodBuilder = new MethodBuilder(_mockClassBuilder.Object, "void", "TestMethod");
        methodBuilder.Parameter<SomeClass>("param1");

        _mockClassBuilder.Verify(m => m.AddRequirement<SomeClass>(), Times.Once());
    }

    [Fact]
    public void TestWithBody_AddsBodyCorrectly()
    {
        var methodBuilder = new MethodBuilder(_mockClassBuilder.Object, "string", "TestMethod");
        methodBuilder.Body(body => body.AddLine("return _thing;"));

        var method = methodBuilder.Build();

        Assert.NotNull(method.Body);
        Assert.NotEmpty(method.Body.Statements);
    }

    [Fact]
    public void TestWithAttributes_AddsAttributesCorrectly()
    {
        var methodBuilder = new MethodBuilder(_mockClassBuilder.Object, "void", "TestMethod");
        methodBuilder.Attributes(attrs => attrs.Add<JsonSerializableAttribute>());

        var method = methodBuilder.Build();

        Assert.NotEmpty(method.AttributeLists);
    }

    [Fact]
    public void TestBuildMethod_BuildsMethodCorrectly()
    {
        var methodBuilder = new MethodBuilder(_mockClassBuilder.Object, "void", "TestMethod");
        methodBuilder.Parameter<int>("param1").Body(body => body.AddLine("return;"));

        var method = methodBuilder.Build();

        Assert.NotNull(method.ParameterList);
        Assert.NotNull(method.Body);
        Assert.NotEmpty(method.Body.Statements);
        Assert.Equal("TestMethod", method.Identifier.ValueText);
    }

    [Fact]
    public void TestMethodProperties_AreSetCorrectly()
    {
        var methodBuilder = new MethodBuilder(_mockClassBuilder.Object, "int", "CalculateSum");
        methodBuilder.Parameter<int>("param1").Parameter<int>("param2");

        var method = methodBuilder.Build();

        Assert.Equal("CalculateSum", method.Identifier.ValueText);
        Assert.Equal("int", method.ReturnType.ToString());
        Assert.Contains(method.Modifiers, m => m.IsKind(SyntaxKind.PublicKeyword));
        Assert.Equal(2, method.ParameterList.Parameters.Count);
        // Assertions for AttributeLists and Body as needed
    }

    // Additional test methods can go here...

    // Consider adding tests for handling invalid cases if applicable.
}
