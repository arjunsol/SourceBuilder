namespace G4ME.SourceBuilder.Syntax;

public class MethodBuilder(ITypeBuilder parent, string returnType, string methodName)
{
    private readonly ITypeBuilder _parentBuilder = parent;
    private readonly List<ParameterSyntax> _parameters = []; //TODO: Implement ParameterBuilder 
    private readonly BlockBuilder _bodyBuilder = new();

    private MethodDeclarationSyntax _methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnType), methodName)
                                                            .AddModifiers(SyntaxFactory.Token(
                                                                SyntaxKind.PublicKeyword));

    public MethodBuilder(ITypeBuilder parentBuilder, string methodName) : this(parentBuilder, "void", methodName)
    {
    }

    public MethodBuilder Parameter<T>(string parameterName)
    {
        var parameterType = TypeName.ValueOf<T>();
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
            .WithType(SyntaxFactory.ParseTypeName(parameterType));
        _parameters.Add(parameter);
        _parentBuilder.AddNamespace<T>();
        
        return this;
    }

    public MethodBuilder Body(Action<BlockBuilder> bodyBuilderAction)
    {
        bodyBuilderAction(_bodyBuilder);
        
        return this;
    }

    //TODO: I'll need a way to parse any type in the statement text likely forcing them to use AddLine<T>
    public MethodBuilder Body(params string[] statements) 
    {
        _bodyBuilder.AddLines(statements);

        return this;
    }

    public MethodBuilder Attributes(Action<AttributeBuilder> attributeConfigurator)
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
