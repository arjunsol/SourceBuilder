using System.Diagnostics.CodeAnalysis;
using G4ME.SourceBuilder.Compile;
using Microsoft.CodeAnalysis;

namespace G4ME.SourceBuilder.Tests.Compile;

[SuppressMessage("Assertions", "xUnit2012:Do not use boolean check to check if a value exists in a collection", Justification = "<Pending>")]
public class RequiredReferencesTests
{
    [Fact]
    public void AddReference_ByType_ShouldAddAssemblyLocation()
    {
        Requirements requirements = new(string.Empty);

        requirements.Add(typeof(object));

        string path = typeof(object).Assembly.Location;

        Assert.True(requirements.References.Any(r => r.FilePath == path));
    }

    [Fact]
    public void AddReference_ByGeneric_ShouldAddAssemblyLocation()
    {
        Requirements requirements = new(string.Empty);

        requirements.Add<object>(); // Add reference to the assembly containing System.Object
        
        string path = typeof(object).Assembly.Location;

        Assert.True(requirements.References.Any(r => r.FilePath == path));
    }

    [Fact]
    public void AddReference_ByString_ShouldAddAssemblyLocation()
    {
        Requirements requirements = new(string.Empty);

        requirements.Add(typeof(object).Assembly.Location); // Add reference to the assembly containing System.Object

        string path = typeof(object).Assembly.Location;

        Assert.True(requirements.References.Any(r => r.FilePath == path));
    }

    [Fact]
    public void AddReference_WithInvalidPath_ShouldThrowFileNotFoundException()
    {
        Requirements requirements = new(string.Empty);

        Assert.Throws<FileNotFoundException>(() => requirements.Add("invalid/path.dll"));
    }

    //[Fact]
    //public void RemoveReference_ShouldRemoveSpecifiedReference()
    //{
    //    Requirements requirements = new(string.Empty);

    //    requirements.Add<object>();
    //    requirements.Remove<object>();

    //    Assert.False(requirements.Contains(typeof(object).Assembly.Location));
    //}

    [Fact]
    public void UnionWith_OtherRequiredReferences_ShouldContainAllReferences()
    {
        Requirements references1 = new(string.Empty);
        references1.Add<object>();

        Requirements references2 = new(string.Empty);
        references2.Add<NamespaceCollection>();

        references1.UnionWith(references2);

        Assert.True(references1.References.Any(r => r.FilePath == typeof(object).Assembly.Location));
        Assert.True(references2.References.Any(r => r.FilePath == typeof(NamespaceCollection).Assembly.Location));
    }

    [Fact]
    public void GetEnumerator_ShouldAllowIterationOverReferences()
    {
        Requirements requirements = new(string.Empty);

        requirements.Add<object>();
        requirements.Add<NamespaceCollection>();

        int count = 0;

        foreach (MetadataReference reference in requirements.References)
        {
            count++;
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public void AddReferences_SameAssembly_ShouldAddSingleReference()
    {
        Requirements requirements = new(string.Empty);

        requirements.Add<object>();
        requirements.Add<string>();

        Assert.Single(requirements.References);
    }
}
