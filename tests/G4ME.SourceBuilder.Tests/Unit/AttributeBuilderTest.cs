﻿using G4ME.SourceBuilder.Tests.Objects;

namespace G4ME.SourceBuilder.Tests.Unit;

public class AttributeBuilderTests
{
    [Fact]
    public void TestAddAttribute_AddsAttributeCorrectly()
    {
        ClassBuilder classBuilder = new("MyClass");
        AttributeBuilder attributeBuilder = new(classBuilder);

        attributeBuilder.Add<SomeAttribute>("arg1", "arg2");

        var attributeList = attributeBuilder.Build();
        var attribute = attributeList.First().Attributes.First();

        Assert.Equal("Some", attribute.Name.ToString());
        Assert.NotNull(attribute.ArgumentList);
        Assert.Equal(2, attribute.ArgumentList.Arguments.Count);
        Assert.Equal("arg1", attribute.ArgumentList.Arguments[0].ToString());
        Assert.Equal("arg2", attribute.ArgumentList.Arguments[1].ToString());
    }

    [Fact]
    public void TestAddAttributeWithNoArguments_AddsAttributeWithoutArguments()
    {
        // TODO: Use MOQ for the typebuilders
        ClassBuilder classBuilder = new("MyClass");
        AttributeBuilder attributeBuilder = new(classBuilder);
        attributeBuilder.Add<SomeAttribute>();

        var attributeList = attributeBuilder.Build();
        var attribute = attributeList.First().Attributes.First();

        Assert.Equal("Some", attribute.Name.ToString());
        Assert.NotNull(attribute.ArgumentList);
        Assert.Empty(attribute.ArgumentList.Arguments);
    }

    [Fact]
    public void TestAddMultipleAttributes_AddsAllAttributes()
    {
        ClassBuilder classBuilder = new("MyClass");
        AttributeBuilder attributeBuilder = new(classBuilder);

        attributeBuilder.Add<SomeAttribute>("arg1");
        attributeBuilder.Add<AnotherAttribute>("arg2", "arg3");

        var attributeList = attributeBuilder.Build();
        Assert.Equal(2, attributeList.Count);

        var firstAttribute = attributeList.First().Attributes.First();
        var secondAttribute = attributeList.Last().Attributes.First();

        Assert.Equal("Some", firstAttribute.Name.ToString());
        Assert.Equal("Another", secondAttribute.Name.ToString());
    }

    [Fact]
    public void TestAddAttributeRemovesAttributeSuffix_RemovesSuffixCorrectly()
    {
        ClassBuilder classBuilder = new("MyClass");
        AttributeBuilder attributeBuilder = new(classBuilder);

        attributeBuilder.Add<SomeAttribute>();

        var attributeList = attributeBuilder.Build();
        var attribute = attributeList.First().Attributes.First();

        Assert.Equal("Some", attribute.Name.ToString());
    }

    [Fact]
    public void TestBuildWithAttributes_BuildsCorrectly()
    {
        ClassBuilder classBuilder = new("MyClass");
        AttributeBuilder attributeBuilder = new(classBuilder);

        attributeBuilder.Add<SomeAttribute>("arg");

        var attributeList = attributeBuilder.Build();

        Assert.Single(attributeList);
    }

    [Fact]
    public void TestBuildWithoutAttributes_ReturnsEmptyList()
    {
        ClassBuilder classBuilder = new("MyClass");
        AttributeBuilder attributeBuilder = new(classBuilder);

        var attributeList = attributeBuilder.Build();

        Assert.Empty(attributeList);
    }

}
