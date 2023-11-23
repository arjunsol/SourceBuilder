using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace G4ME.SourceBuilder.Tests.Unit;

public class PropertyBuilderTests
{
    [Fact]
    public void AddProperty_ShouldAddPropertyWithDefaultModifier()
    {
        var builder = new PropertyBuilder();
        builder.Add<int>("MyProperty");

        var properties = builder.Build();

        Assert.Single(properties);
        Assert.Equal("MyProperty", properties[0].Identifier.ValueText);
        Assert.Contains(properties[0].Modifiers, m => m.IsKind(SyntaxKind.PublicKeyword));
    }

    [Fact]
    public void AddPrivateProperty_ShouldAddPrivateProperty()
    {
        var builder = new PropertyBuilder();
        builder.AddPrivate<int>("MyPrivateProperty");

        var properties = builder.Build();

        Assert.Single(properties);
        Assert.Equal("MyPrivateProperty", properties[0].Identifier.ValueText);
        Assert.Contains(properties[0].Modifiers, m => m.IsKind(SyntaxKind.PrivateKeyword));
    }

    [Fact]
    public void AddGetProperty_ShouldAddPropertyWithGetter()
    {
        var builder = new PropertyBuilder();
        builder.Add<int>("MyProperty").Get();

        var properties = builder.Build();

        Assert.Single(properties);
        
        var property = properties[0];
        Assert.NotNull(property.AccessorList);
        Assert.Single(property.AccessorList.Accessors);
        Assert.True(property.AccessorList.Accessors[0].IsKind(SyntaxKind.GetAccessorDeclaration));
    }

    [Fact]
    public void AddSetProperty_ShouldAddPropertyWithSetter()
    {
        var builder = new PropertyBuilder();
        builder.Add<int>("MyProperty").Set();

        var properties = builder.Build();

        Assert.Single(properties);

        var property = properties[0];
        Assert.Equal("int", property.Type.ToString());
        Assert.NotNull(property.AccessorList);
        Assert.Single(property.AccessorList.Accessors);
        Assert.True(property.AccessorList.Accessors[0].IsKind(SyntaxKind.SetAccessorDeclaration));
    }

    [Fact]
    public void AddPrivateSetProperty_ShouldAddPropertyWithPrivateSetter()
    {
        var builder = new PropertyBuilder();
        builder.Add<int>("MyProperty").PrivateSet();

        var properties = builder.Build();

        Assert.Single(properties);

        var property = properties[0];
        Assert.NotNull(property.AccessorList);

        var setter = property.AccessorList.Accessors[0];
        Assert.True(setter.IsKind(SyntaxKind.SetAccessorDeclaration));
        Assert.Contains(setter.Modifiers, m => m.IsKind(SyntaxKind.PrivateKeyword));
    }

    [Fact]
    public void Build_ShouldReturnAllAddedProperties()
    {
        var builder = new PropertyBuilder();
        builder.Add<int>("PropertyOne").Get();
        builder.AddPrivate<int>("PropertyTwo").Set();

        var properties = builder.Build();

        Assert.Equal(2, properties.Length);
    }
}
