using G4ME.SourceBuilder.Tests.Objects;

namespace G4ME.SourceBuilder.Tests.Unit;

public class ClassBuilderTests
{
    [Fact]
    public void TestEmptyConstructor_SetsClassName()
    {
        var className = "TestClassName";
        var classBuilder = new ClassBuilder(className);

        Assert.Equal(className, classBuilder.TypeName);
        Assert.Empty(classBuilder.Namespace);
    }

    [Fact]
    public void TestConstructorWithNamespace_SetsClassNameAndNamespace()
    {
        var className = "TestClassName";
        var classNamespace = "TestNamespace";
        var classBuilder = new ClassBuilder(className, classNamespace);

        Assert.Equal(className, classBuilder.TypeName);
        Assert.Equal(classNamespace, classBuilder.Namespace);
    }

    [Fact]
    public void TestAddParameterGenericReturn_AddsCorrectNamespace()
    {
        var classBuilder = new ClassBuilder("TestClass");

        classBuilder.AddMethod<List<SomeClass>>("TestMethod",
                     m => m.Body(
                     b => b.AddLine("return new List<TestType>();")));
        
        var nspace = classBuilder.GetRequirements().Namespaces;

        Assert.Equal(2, nspace.Count());
        Assert.Contains("System.Collections.Generic", nspace);
        Assert.Contains("G4ME.SourceBuilder.Tests.Objects", nspace);
    }

    [Fact]
    public void TestInheritsFromValidBaseClass_AddsBaseClass()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.Extends<SomeBaseClass>();

        var classDeclaration = classBuilder.Build();

        Assert.NotNull(classDeclaration.BaseList);

        var baseType = classDeclaration.BaseList.Types.Single();

        Assert.Equal(nameof(SomeBaseClass), baseType.ToString());
    }

    [Fact]
    public void TestImplementsInterface_ValidInterface_AddsInterface()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.Implements<ISomeInterface>();

        var classDeclaration = classBuilder.Build();

        Assert.NotNull(classDeclaration.BaseList);
        Assert.Contains(classDeclaration.BaseList.Types, t => t.ToString() == nameof(ISomeInterface));
    }

    [Fact]
    public void TestImplementsInterface_GenericInterface_AddsInterface()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.Implements<ISomeGenericInterface<SomeClass>>();

        var classDeclaration = classBuilder.Build();

        Assert.NotNull(classDeclaration.BaseList);
        Assert.Contains(classDeclaration.BaseList.Types, t => t.ToString() == "ISomeGenericInterface<SomeClass>");        
    }

    [Fact]
    public void TestWithProperty_AddsPropertyCorrectly()
    {
        var classBuilder = new ClassBuilder("TestClass");
        
        classBuilder.Properties(p =>
        {
            p.Add<int>("TestProperty").Get().Set();
                          //.WithAttributes(attributeBuilder =>
                          //{
                          //    attributeBuilder.Add<SomeAttribute>();
                          //    // Additional attribute configurations
                          //}); //TODO: Add attributes to properties
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
        classBuilder.AddMethod("TestMethod", methodBuilder =>
        {
            methodBuilder.Parameter<int>("param1").Body(bodyBuilder =>
            {
                bodyBuilder.AddLine("return param1;");
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
        classBuilder.Constructor(constructorBuilder =>
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
        classBuilder.Attributes(attributeBuilder =>
        {
            attributeBuilder.Add<SomeAttribute>();
            // Additional attribute configuration as needed
        });

        var classDeclaration = classBuilder.Build();

        Assert.NotEmpty(classDeclaration.AttributeLists);
        var attribute = classDeclaration.AttributeLists.SelectMany(a => a.Attributes)
                         .FirstOrDefault(a => a.Name.ToString() == "Some");

        Assert.NotNull(attribute);
        // Additional assertions for the attribute's arguments, etc.
    }

    [Fact]
    public void TestAddDuplicateConstructor_ThrowsException()
    {
        var classBuilder = new ClassBuilder("TestClass");

        classBuilder.Constructor();
        Assert.Throws<InvalidOperationException>(classBuilder.Constructor);
    }

    [Fact]
    public void TestAddTwoConstrcutors_DifferentParameters_AddsBoth()
    {
        var classBuilder = new ClassBuilder("TestClass");

        classBuilder.Constructor(c => c.Parameter<string>("firstName").Parameter<string>("lastName"));
        classBuilder.Constructor(c => c.Parameter<int>("age"));

        var classDeclaration = classBuilder.Build();

        Assert.Equal(2, classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().Count());
    }

    [Fact]
    public void TestAddTwoConstructors_SameParameterCount_AddsBoth()
    {
        var classBuilder = new ClassBuilder("TestClass");

        classBuilder.Constructor(c => c.Parameter<string>("name"));
        classBuilder.Constructor(c => c.Parameter<int>("age"));

        var classDeclaration = classBuilder.Build();

        Assert.Equal(2, classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().Count());
    }

    [Fact]
    public void TestAddDuplicateConstructor_WithParameters_ThrowsException()
    {
        var classBuilder = new ClassBuilder("TestClass");

        classBuilder.Constructor(c => c.Parameter<string>("name"));
        Assert.Throws<InvalidOperationException>(() => classBuilder.Constructor(c => c.Parameter<string>("name")));
    }

    [Fact]
    public void ClassBuilder_ImplementsClass_ThrowsException()
    {
        var builder = new ClassBuilder("MyRecord");

        Assert.Throws<ArgumentException>(builder.Implements<SomeClass>);
    }

}

