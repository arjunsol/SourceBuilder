using G4ME.SourceBuilder.Compile;

namespace G4ME.SourceBuilder.Syntax;

public interface ITypeBuilder

{
    string Namespace { get; }
    string TypeName { get; }
    TypeDeclarationSyntax Build();
    void AddRequirement<T>();
    Requirements GetRequirements();    
}
