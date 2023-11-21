namespace G4ME.SourceBuilder.Syntax;

public class MethodBuilder(ITypeBuilder parent, string returnType, string methodName)
{
    private readonly ITypeBuilder _parentBuilder = parent;
    private readonly List<ParameterSyntax> _parameters = [];
    private readonly BlockBuilder _bodyBuilder = new();

    private MethodDeclarationSyntax _methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnType), methodName)
                                                            .AddModifiers(SyntaxFactory.Token(
                                                                SyntaxKind.PublicKeyword));

    public MethodBuilder(ITypeBuilder parentBuilder, string methodName) : this(parentBuilder, "void", methodName)
    {
    }

    public MethodBuilder Parameter<T>(string parameterName)
    {
        var parameterType = typeof(T).Name;
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
            .WithType(SyntaxFactory.ParseTypeName(parameterType));
        _parameters.Add(parameter);
        _parentBuilder.AddNamespace<T>();
        return this;
    }

    public MethodBuilder WithBody(Action<BlockBuilder> bodyBuilderAction)
    {
        bodyBuilderAction(_bodyBuilder);
        return this;
    }

    public MethodBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator)
    {
        var attributeBuilder = new AttributeBuilder();
        attributeConfigurator(attributeBuilder);
        _methodDeclaration = _methodDeclaration.WithAttributeLists(attributeBuilder.Build());
        return this;
    }

    public MethodDeclarationSyntax Build()
    {
        _methodDeclaration = _methodDeclaration
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(_parameters)))
            .WithBody(_bodyBuilder.Build());
        
        return _methodDeclaration;
    }

    public ITypeBuilder EndMethod() => _parentBuilder;
}
