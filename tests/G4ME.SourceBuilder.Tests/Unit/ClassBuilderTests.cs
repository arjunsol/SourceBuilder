using G4ME.SourceBuilder.Tests.Objects;

namespace G4ME.SourceBuilder.Tests.Unit;

public class ClassBuilderTests
{
    [Fact]
    public void TestEmptyConstructor_SetsClassName()
    {
        var className = "TestClassName";
        var classBuilder = new ClassBuilder(className);

        Assert.Equal(className, classBuilder.ClassName);
        Assert.Empty(classBuilder.Namespace);
    }

    [Fact]
    public void TestConstructorWithNamespace_SetsClassNameAndNamespace()
    {
        var className = "TestClassName";
        var classNamespace = "TestNamespace";
        var classBuilder = new ClassBuilder(className, classNamespace);

        Assert.Equal(className, classBuilder.ClassName);
        Assert.Equal(classNamespace, classBuilder.Namespace);
    }

    [Fact]
    public void TestAddParameterGenericReturn_AddsCorrectNamespace()
    {
        var classBuilder = new ClassBuilder("TestClass")
                               .WithMethod<List<SomeClass>>("TestMethod",
                                                           m => m.WithBody(
                                                           b => b.AddStatement("return new List<TestType>();")));
        var nspace = classBuilder.GetRequiredNamespaces();

        Assert.Equal(2, nspace.Count());
        Assert.Contains("System.Collections.Generic", nspace);
        Assert.Contains("G4ME.SourceBuilder.Tests.Objects", nspace);
    }

    [Fact]
    public void TestInheritsFromValidBaseClass_AddsBaseClass()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.InheritsFrom<SomeBaseClass>();

        var classDeclaration = classBuilder.Build();

        Assert.NotNull(classDeclaration.BaseList);

        var baseType = classDeclaration.BaseList.Types.Single();

        Assert.Equal(nameof(SomeBaseClass), baseType.ToString());
    }

    [Fact]
    public void TestImplementsInterface_ValidInterface_AddsInterface()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.ImplementsInterface<ISomeInterface>();

        var classDeclaration = classBuilder.Build();

        Assert.NotNull(classDeclaration.BaseList);
        Assert.Contains(classDeclaration.BaseList.Types, t => t.ToString() == nameof(ISomeInterface));
    }

    [Fact]
    public void TestWithProperty_AddsPropertyCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.WithProperty<int>("TestProperty", propertyBuilder =>
        {
            propertyBuilder.WithGetter().WithSetter()
                          .WithAttributes(attributeBuilder =>
                          {
                              attributeBuilder.AddAttribute<SomeAttribute>();
                              // Additional attribute configurations
                          });
        });

        var classDeclaration = classBuilder.Build();
        var propertyDeclaration = classDeclaration.Members
            .OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault(p => p.Identifier.ValueText == "TestProperty");

        Assert.NotNull(propertyDeclaration);
        Assert.Equal("int", propertyDeclaration.Type.ToString());
        Assert.NotNull(propertyDeclaration.AccessorList);
        Assert.Contains(propertyDeclaration.AccessorList.Accessors, a => a.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.GetAccessorDeclaration);
        Assert.Contains(propertyDeclaration.AccessorList.Accessors, a => a.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.SetAccessorDeclaration);
        //Assert.Contains(propertyDeclaration.AttributeLists, al => al.Attributes.Any(a => a.Name.ToString() == "Custom"));
    }

    [Fact]
    public void TestWithMethod_AddsMethodCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.WithMethod("TestMethod", methodBuilder =>
        {
            methodBuilder.Parameter<int>("param1").WithBody(bodyBuilder =>
            {
                bodyBuilder.AddStatement("return param1;");
            });
        });

        var classDeclaration = classBuilder.Build();
        var methodDeclaration = classDeclaration.Members
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == "TestMethod");

        Assert.NotNull(methodDeclaration);
        Assert.Equal("TestMethod", methodDeclaration.Identifier.ValueText);
        Assert.Single(methodDeclaration.ParameterList.Parameters);
        Assert.Equal("param1", methodDeclaration.ParameterList.Parameters[0].Identifier.ValueText);

        Assert.NotNull(methodDeclaration.ParameterList);
        Assert.NotEmpty(methodDeclaration.ParameterList.Parameters);

        var firstParameter = methodDeclaration.ParameterList.Parameters[0];
        Assert.NotNull(firstParameter);
        Assert.NotNull(firstParameter.Type);

        // Now it's safe to call ToString() since we've asserted that Type is not null
        Assert.Equal("int", firstParameter.Type.ToString());

        Assert.NotNull(methodDeclaration.Body);
        // Additional assertions can be made about the method's body, return type, attributes, etc.
    }

    [Fact]
    public void TestWithConstructor_AddsConstructorCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.WithConstructor(constructorBuilder =>
        {
            constructorBuilder.Parameter<int>("param1");
            // Additional constructor configuration as needed
        });

        var classDeclaration = classBuilder.Build();
        var constructorDeclaration = classDeclaration.Members
            .OfType<ConstructorDeclarationSyntax>()
            .FirstOrDefault();

        Assert.NotNull(constructorDeclaration);
        Assert.Single(constructorDeclaration.ParameterList.Parameters);
        Assert.Equal("param1", constructorDeclaration.ParameterList.Parameters[0].Identifier.ValueText);

        var firstParameter = constructorDeclaration.ParameterList.Parameters[0];
        Assert.NotNull(firstParameter);
        Assert.NotNull(firstParameter.Type);

        // Now it's safe to call ToString() since we've asserted that Type is not null
        Assert.Equal("int", firstParameter.Type.ToString());
    }

    [Fact]
    public void TestWithAttributes_AddsAttributesCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.WithAttributes(attributeBuilder =>
        {
            attributeBuilder.AddAttribute<SomeAttribute>();
            // Additional attribute configuration as needed
        });

        var classDeclaration = classBuilder.Build();

        Assert.NotEmpty(classDeclaration.AttributeLists);
        var attribute = classDeclaration.AttributeLists.SelectMany(a => a.Attributes)
                         .FirstOrDefault(a => a.Name.ToString() == "Some");

        Assert.NotNull(attribute);
        // Additional assertions for the attribute's arguments, etc.
    }

}
