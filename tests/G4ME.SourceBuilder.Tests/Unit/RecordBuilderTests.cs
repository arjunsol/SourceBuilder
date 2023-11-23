using G4ME.SourceBuilder.Tests.Objects;
using G4ME.SourceBuilder.Types;

namespace G4ME.SourceBuilder.Tests.Unit;

public class RecordBuilderTests
{
    [Fact]
    public void RecordBuilder_CreatesRecordWithCorrectName()
    {
        var builder = new RecordBuilder("MyRecord");
        var recordDeclaration = builder.Build() as RecordDeclarationSyntax;

        Assert.NotNull(recordDeclaration);
        Assert.Equal("MyRecord", recordDeclaration.Identifier.ValueText);
    }

    [Fact]
    public void RecordBuilder_CreatesRecordWithCorrectNamespace()
    {
        var builder = new RecordBuilder("MyRecord", "MyNamespace");
        var recordDeclaration = builder.Build() as RecordDeclarationSyntax;

        Assert.NotNull(recordDeclaration);
        Assert.Equal("MyNamespace", builder.Namespace);
    }

    [Fact]
    public void RecordBuilder_InheritsFromBaseRecord()
    {
        var builder = new RecordBuilder("MyRecord").Extends<BaseRecord>();

        var recordDeclaration = builder.Build();

        Assert.NotNull(recordDeclaration.BaseList);
        Assert.Contains(recordDeclaration.BaseList.Types,
                        t => t.Type.ToString() == nameof(BaseRecord));
    }

    [Fact]
    public void RecordBuilder_ImplementsInterface()
    {
        var builder = new RecordBuilder("MyRecord").Implements<ISomeInterface>();

        var recordDeclaration = builder.Build();

        Assert.NotNull(recordDeclaration.BaseList);
        Assert.Contains(recordDeclaration.BaseList.Types,
                        t => t.Type.ToString() == nameof(ISomeInterface));
    }

    // TODO: Bodies for records (check other commented out tests
    //[Fact]
    //public void RecordBuilder_AddsPropertyCorrectly()
    //{
    //    var builder = new RecordBuilder("MyRecord")
    //        .Properties(p => p.Add<int>("MyProperty"));

    //    var recordDeclaration = builder.Build();
    //    var property = recordDeclaration.Members
    //        .OfType<PropertyDeclarationSyntax>()
    //        .FirstOrDefault(p => p.Identifier.ValueText == "MyProperty");

    //    Assert.NotNull(property);
    //    Assert.Equal("int", property.Type.ToString());
    //}

    //[Fact]
    //public void TestAddParameterGenericReturn_AddsCorrectNamespace()
    //{
    //    var classBuilder = new RecordBuilder("TestClass")
    //                           .AddMethod<List<SomeClass>>("TestMethod",
    //                                                       m => m.Body(
    //                                                       b => b.AddLine("return new List<TestType>();")));
    //    var nspace = classBuilder.GetRequiredNamespaces();

    //    Assert.Equal(2, nspace.Count());
    //    Assert.Contains("System.Collections.Generic", nspace);
    //    Assert.Contains("G4ME.SourceBuilder.Tests.Objects", nspace);
    //}

    //[Fact]
    //public void RecordBuilder_AddsMethodCorrectly()
    //{
    //    var builder = new RecordBuilder("MyRecord")
    //        .AddMethod("MyMethod", mb => mb.Parameter<int>("param1"));

    //    var recordDeclaration = builder.Build();
    //    var method = recordDeclaration.Members
    //        .OfType<MethodDeclarationSyntax>()
    //        .FirstOrDefault(m => m.Identifier.ValueText == "MyMethod");

    //    Assert.NotNull(method);
    //    Assert.Single(method.ParameterList.Parameters);
    //    Assert.Equal("param1", method.ParameterList.Parameters[0].Identifier.ValueText);
    //}

    [Fact]
    public void RecordBuilder_AddsAttributesCorrectly()
    {
        var builder = new RecordBuilder("MyRecord")
            .Attributes(ab => ab.Add<SomeAttribute>());

        var recordDeclaration = builder.Build();

        Assert.NotEmpty(recordDeclaration.AttributeLists);
        var attribute = recordDeclaration.AttributeLists.SelectMany(a => a.Attributes)
            .FirstOrDefault(a => a.Name.ToString() == "Some");
        Assert.NotNull(attribute);
    }

    [Fact]
    public void RecordBuilder_ImplementsClass_ThrowsException()
    {
        var builder = new RecordBuilder("MyRecord");

        Assert.Throws<ArgumentException>(() => builder.Implements<SomeClass>());
    }
}
