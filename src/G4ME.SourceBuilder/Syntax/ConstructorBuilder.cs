using System.Reflection.Emit;

namespace G4ME.SourceBuilder.Syntax;

public class ConstructorBuilder(ClassBuilder parent)
{
    private readonly ParameterBuilder _parameterBuilder = new();
    private readonly BlockBuilder _bodyBuilder = new();

    private ConstructorDeclarationSyntax _constructorDeclaration = SyntaxFactory.ConstructorDeclaration(parent.TypeName)
                                                                   .AddModifiers(SyntaxFactory.Token(
                                                                       SyntaxKind.PublicKeyword));

    public ConstructorBuilder Parameter<T>(string parameterName)
    {
        _parameterBuilder.AddParameter<T>(parameterName);
        parent.AddNamespace<T>();
        
        return this;
    }

    public ConstructorBuilder Body(params string[] statements)
    {
        _bodyBuilder.AddLines(statements);
        
        return this;
    }

    public ConstructorBuilder Body(Action<BlockBuilder> bodyBuilderAction)
    {
        bodyBuilderAction(_bodyBuilder);
        return this;
    }

    public ConstructorDeclarationSyntax Build()
    {
        _constructorDeclaration = _constructorDeclaration
            .WithParameterList(_parameterBuilder.Build())
            .WithBody(_bodyBuilder.Build());

        return _constructorDeclaration;
    }

    //public ClassBuilder EndConstructor() => _parentBuilder;
}
