namespace G4ME.SourceBuilder.Syntax;

public class ConstructorBuilder(ClassBuilder parent)
{
    private readonly ClassBuilder _parentBuilder = parent;
    private readonly List<ParameterSyntax> _parameters = [];
    private readonly BlockBuilder _bodyBuilder = new();

    private ConstructorDeclarationSyntax _constructorDeclaration = SyntaxFactory.ConstructorDeclaration(parent.ClassName)
                                                                   .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

    public ConstructorBuilder Parameter<T>(string parameterName)
    {
        var parameterType = TypeName.ValueOf<T>();
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
            .WithType(SyntaxFactory.ParseTypeName(parameterType));
        _parameters.Add(parameter);
        _parentBuilder.AddNamespace<T>();
        return this;
    }

    public ConstructorBuilder WithBody(Action<BlockBuilder> bodyBuilderAction)
    {
        bodyBuilderAction(_bodyBuilder);
        return this;
    }

    public ConstructorDeclarationSyntax Build()
    {
        _constructorDeclaration = _constructorDeclaration
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(_parameters)))
            .WithBody(_bodyBuilder.Build());
        return _constructorDeclaration;
    }

    public ClassBuilder EndConstructor() => _parentBuilder;

}
