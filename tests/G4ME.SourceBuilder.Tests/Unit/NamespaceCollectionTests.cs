﻿using G4ME.SourceBuilder.Tests.Objects;

namespace G4ME.SourceBuilder.Tests.Unit;

public class NamespaceCollectionTests
{
    [Fact]
    public void Add_ValidNamespace_AddsNamespace()
    {
        var collection = new NamespaceCollection("");
        collection.Add<SomeClass>();

        Assert.Contains("G4ME.SourceBuilder.Tests.Objects", collection.GetAll());
    }

    [Fact]
    public void Add_DuplicateNamespace_IgnoresDuplicate()
    {
        var collection = new NamespaceCollection("");
        collection.Add<SomeClass>();
        collection.Add<SomeClass>(); // Duplicate

        Assert.Single(collection.GetAll(), ns => ns == "G4ME.SourceBuilder.Tests.Objects");
    }

    [Fact]
    public void Add_PrimitiveType_DoesNotAddNamespace()
    {
        var collection = new NamespaceCollection("");
        collection.Add<int>(); // Primitive type

        Assert.Empty(collection.GetAll());
    }

    [Fact]
    public void Add_GenericType_AddsAllRelevantNamespaces()
    {
        var collection = new NamespaceCollection("");
        collection.Add<Tuple<string, int>>();

        Assert.Contains("System", collection.GetAll());
    }

    [Fact]
    public void Add_TypeWithCurrentNamespace_DoesNotAddNamespace()
    {
        var currentNamespace = "G4ME.SourceBuilder";
        var collection = new NamespaceCollection(currentNamespace);
        collection.Add<NamespaceCollection>(); // Assuming NamespaceCollection is in currentNamespace

        Assert.DoesNotContain(currentNamespace, collection.GetAll());
    }
}