namespace G4ME.SourceBuilder;

public interface ITypeBuilder

{
    string Namespace { get; }
    string TypeName { get; }

    IEnumerable<string> GetRequiredNamespaces();
    TypeDeclarationSyntax Build();
    void AddNamespace<T>();
}
