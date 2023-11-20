namespace G4ME.SourceBuilder;

public interface ITypeBuilder

{
    string Namespace { get; }
    string ClassName { get; }

    IEnumerable<string> GetRequiredNamespaces();
    TypeDeclarationSyntax Build();
}
