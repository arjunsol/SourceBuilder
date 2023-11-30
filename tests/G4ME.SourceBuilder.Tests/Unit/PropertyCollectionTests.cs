using Microsoft.CodeAnalysis.CSharp;

namespace G4ME.SourceBuilder.Tests.Unit;

public class PropertyCollectionTests
{
    [Fact]
    public void AddProperty_ShouldAddProperty()
    {
        var collection = new PropertyCollection();
        var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "TestProperty");

        collection.AddProperty(property);

        Assert.Equal(property, collection.CurrentProperty);
    }

    [Fact]
    public void CurrentProperty_ShouldReturnLastAddedProperty()
    {
        var collection = new PropertyCollection();
        var property1 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "Property1");
        var property2 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "Property2");
        collection.AddProperty(property1);
        collection.AddProperty(property2);

        Assert.Equal(property2, collection.CurrentProperty);
    }

    [Fact]
    public void CurrentProperty_WithoutAddingProperty_ShouldThrowException()
    {
        var collection = new PropertyCollection();

        Assert.Throws<InvalidOperationException>(() => collection.CurrentProperty);
    }

    [Fact]
    public void UpdateCurrentProperty_ShouldUpdateProperty()
    {
        var collection = new PropertyCollection();
        var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "OriginalProperty");
        collection.AddProperty(property);
        var updatedProperty = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "UpdatedProperty");

        collection.UpdateCurrentProperty(updatedProperty);

        Assert.Equal(updatedProperty, collection.CurrentProperty);
    }

    [Fact]
    public void UpdateCurrentProperty_WithoutAddingProperty_ShouldThrowException()
    {
        var collection = new PropertyCollection();
        var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "Property");

        Assert.Throws<InvalidOperationException>(() => collection.UpdateCurrentProperty(property));
    }

    [Fact]
    public void Build_ShouldReturnAllAddedProperties()
    {
        var collection = new PropertyCollection();
        var property1 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "Property1");
        var property2 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "Property2");
        collection.AddProperty(property1);
        collection.AddProperty(property2);

        var properties = collection.Build();

        Assert.Contains(property1, properties);
        Assert.Contains(property2, properties);
        Assert.Equal(2, properties.Length);
    }
}
