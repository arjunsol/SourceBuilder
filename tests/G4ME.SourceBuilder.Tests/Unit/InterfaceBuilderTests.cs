using G4ME.SourceBuilder.Tests.Objects;
using G4ME.SourceBuilder.Types;

namespace G4ME.SourceBuilder.Tests.Unit;

public class InterfaceBuilderTests
{
    [Fact]
    public void TestConstructorWithNamespace_SetsInterfaceNameAndNamespace()
    {
        var interfaceName = "TestInterfaceName";
        var interfaceNamespace = "TestNamespace";
        var interfaceBuilder = new InterfaceBuilder(interfaceName, interfaceNamespace);

        Assert.Equal(interfaceName, interfaceBuilder.TypeName);
        Assert.Equal(interfaceNamespace, interfaceBuilder.Namespace);
    }

    [Fact]
    public void InterfaceBuilder_CreatesInterfaceWithCorrectName()
    {
        var builder = new InterfaceBuilder("IMyInterface");
        var interfaceDeclaration = builder.Build() as InterfaceDeclarationSyntax;

        Assert.NotNull(interfaceDeclaration);
        Assert.Equal("IMyInterface", interfaceDeclaration.Identifier.ValueText);
    }

    [Fact]
    public void InterfaceBuilder_AddsMethodCorrectly()
    {
        var builder = new InterfaceBuilder("IMyInterface");
        builder.AddMethod("TestMethod", mb => mb.Parameter<int>("param1"));

        var interfaceDeclaration = builder.Build();

        var method = interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.ValueText == "TestMethod");

        Assert.NotNull(method);
        Assert.Single(method.ParameterList.Parameters);
        Assert.Equal("param1", method.ParameterList.Parameters[0].Identifier.ValueText);
    }

    [Fact]
    public void InterfaceBuilder_AddsPropertyCorrectly()
    {
        var builder = new InterfaceBuilder("IMyInterface");
        builder.Properties(pb => pb.Add<int>("TestProperty"));


        var interfaceDeclaration = builder.Build();

        var property = interfaceDeclaration.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault(p => p.Identifier.ValueText == "TestProperty");

        Assert.NotNull(property);
        Assert.Equal("int", property.Type.ToString());
    }

    [Fact]
    public void InterfaceBuilder_AddsAttributesCorrectly()
    {
        var builder = new InterfaceBuilder("IMyInterface");
        
        builder.Attributes(ab => ab.Add<SomeAttribute>());

        var interfaceDeclaration = builder.Build();

        Assert.NotEmpty(interfaceDeclaration.AttributeLists);
        var attribute = interfaceDeclaration.AttributeLists.SelectMany(a => a.Attributes).FirstOrDefault(a => a.Name.ToString() == "Some");
        Assert.NotNull(attribute);
    }

    [Fact]
    public void InterfaceBuilder_InheritsFromBaseInterfaceCorrectly()
    {
        var builder = new InterfaceBuilder("IMyInterface");
        builder.Extends<IBaseInterface>();

        var interfaceDeclaration = builder.Build();

        Assert.NotNull(interfaceDeclaration.BaseList);
        Assert.Single(interfaceDeclaration.BaseList.Types);
        Assert.Equal("IBaseInterface", interfaceDeclaration.BaseList.Types[0].Type.ToString());
    }

    [Fact]
    public void InterfaceBuilder_InheritFromClass_ThrowsException()
    {
        var builder = new InterfaceBuilder("IMyInterface");        
        
        Assert.Throws<ArgumentException>(() => builder.Extends<SomeClass>());        
    }
}

// Mock interface used in the tests
interface IBaseInterface { }
