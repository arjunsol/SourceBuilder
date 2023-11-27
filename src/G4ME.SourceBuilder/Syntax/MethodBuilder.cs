namespace G4ME.SourceBuilder.Syntax;

public class MethodBuilder(ITypeBuilder parent, string returnType, string methodName)
{
    private readonly ParameterBuilder _parameterBuilder = new();

    private readonly BlockBuilder _bodyBuilder = new();

    private MethodDeclarationSyntax _methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnType), methodName)
                                                            .AddModifiers(SyntaxFactory.Token(
                                                                SyntaxKind.PublicKeyword));
    public MethodBuilder(ITypeBuilder parentBuilder, string methodName) : this(parentBuilder, "void", methodName)
    {
    }

    public MethodBuilder Parameter<T>(string parameterName)
    {
        _parameterBuilder.AddParameter<T>(parameterName);

        parent.AddRequirement<T>();

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
        AttributeBuilder attributeBuilder = new(parent);
        attributeConfigurator(attributeBuilder);
        _methodDeclaration = _methodDeclaration.WithAttributeLists(attributeBuilder.Build());
        return this;
    }

    public MethodDeclarationSyntax Build()
    {
        _methodDeclaration = _methodDeclaration
            .WithParameterList(_parameterBuilder.Build())
            .WithBody(_bodyBuilder.Build());
        
        return _methodDeclaration;
    }
}
