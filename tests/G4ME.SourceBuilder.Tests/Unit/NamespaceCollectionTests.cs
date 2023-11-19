using G4ME.SourceBuilder.Compile;
using G4ME.SourceBuilder.Tests.Objects;

namespace G4ME.SourceBuilder.Tests.Unit;

public class NamespaceCollectionTests
{
    [Fact]
    public void Add_ValidNamespace_AddsNamespace()
    {
        var collection = new Requirements("");
        collection.Add<SomeClass>();

        Assert.Contains("G4ME.SourceBuilder.Tests.Objects", collection.Namespaces);
    }

    [Fact]
    public void Add_DuplicateNamespace_IgnoresDuplicate()
    {
        var collection = new Requirements("");
        collection.Add<SomeClass>();
        collection.Add<SomeClass>(); // Duplicate

        Assert.Single(collection.Namespaces, ns => ns == "G4ME.SourceBuilder.Tests.Objects");
    }

    [Fact(Skip = "Unsure if we need this")]
    public void Add_PrimitiveType_DoesNotAddNamespace()
    {
        var collection = new Requirements("");
        collection.Add<int>(); // Primitive type

        Assert.Empty(collection.Namespaces);
    }

    [Fact]
    public void Add_GenericType_AddsAllRelevantNamespaces()
    {
        var collection = new Requirements("");
        collection.Add<Tuple<string, int>>();

        Assert.Contains("System", collection.Namespaces);
    }

    [Fact]
    public void Add_TypeWithCurrentNamespace_DoesNotAddNamespace()
    {
        var currentNamespace = "G4ME.SourceBuilder";
        var collection = new Requirements(currentNamespace);
        collection.Add<NamespaceCollection>(); // Assuming NamespaceCollection is in currentNamespace

        Assert.DoesNotContain(currentNamespace, collection.Namespaces);
    }
}
