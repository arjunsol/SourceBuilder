namespace G4ME.SourceBuilder.Tests;

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
        Assert.Equal("Int32", parameter.Type.ToString());
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
        constructorBuilder.Parameter<SomeType>("param1");

        Assert.Contains(typeof(SomeType).Namespace, classBuilder.GetRequiredNamespaces());
    }

    [Fact]
    public void TestWithBody_AddsBodyCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        var constructorBuilder = new ConstructorBuilder(classBuilder);
        constructorBuilder.WithBody(body => { /* body configuration */ });

        var constructor = constructorBuilder.Build();

        // Assertions for the body content, depending on how BlockBuilder is implemented
    }

    [Fact]
    public void TestBuildConstructor_BuildsConstructorCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        var constructorBuilder = new ConstructorBuilder(classBuilder);
        constructorBuilder.Parameter<int>("param1").WithBody(body => { /* body configuration */ });

        var constructor = constructorBuilder.Build();

        Assert.NotNull(constructor.ParameterList);
        Assert.NotNull(constructor.Body);
    }

    [Fact]
    public void TestEndConstructorReturnsParentBuilder_ReturnsParentBuilder()
    {
        var classBuilder = new ClassBuilder("TestClass");
        var constructorBuilder = new ConstructorBuilder(classBuilder);

        var returnedBuilder = constructorBuilder.EndConstructor();

        Assert.Equal(classBuilder, returnedBuilder);
    }
}
