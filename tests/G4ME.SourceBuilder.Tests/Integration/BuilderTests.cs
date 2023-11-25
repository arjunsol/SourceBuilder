namespace G4ME.SourceBuilder.Tests.Integration;

// TODO: Add tests for using statments being correct (verifier maybe?)
public class BuilderTests
{
    [Fact]
    public void Builder_CreatesCompilationUnitWithMultipleTypes()
    {
        var builder = new Builder("MyNamespace")
            .AddClass("MyClass", c => c.AddMethod("MyMethod", m => m.Parameter<int>("param")))
            .AddInterface("IMyInterface", i => i.AddMethod("InterfaceMethod", m => { }))
            .AddRecord("MyRecord", r => r.Parameter<string>("MyParameter"));

        var compilationUnit = builder.Build();

        Assert.NotNull(compilationUnit);
        Assert.Single(compilationUnit.Members);
        Assert.IsType<FileScopedNamespaceDeclarationSyntax>(compilationUnit.Members[0]);

        var namespaceDeclaration = (FileScopedNamespaceDeclarationSyntax)compilationUnit.Members[0];
        Assert.Equal(3, namespaceDeclaration.Members.Count);
        Assert.Contains(namespaceDeclaration.Members, m => m is ClassDeclarationSyntax syntax && syntax.Identifier.ValueText == "MyClass");
        Assert.Contains(namespaceDeclaration.Members, m => m is InterfaceDeclarationSyntax syntax && syntax.Identifier.ValueText == "IMyInterface");
        Assert.Contains(namespaceDeclaration.Members, m => m is RecordDeclarationSyntax syntax && syntax.Identifier.ValueText == "MyRecord");
    }

    [Fact]
    public void Builder_AddClass_ReturnsCodeContainingCorrectStrings()
    {
        var builder = new Builder("MyNamespace")
            .AddClass("MyClass", c => c
                .AddMethod("MyMethod", m => m
                    .Parameter<int>("param")));
        
        var codeString = builder.ToString();
        Assert.NotNull(codeString);
        Assert.Contains("namespace MyNamespace", codeString);
        Assert.Contains("public class MyClass", codeString);
        Assert.Contains("public void MyMethod(int param)", codeString);
    }
}
