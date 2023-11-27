namespace G4ME.SourceBuilder.Syntax;

public class ConstructorBuilder(ClassBuilder parent)
{
    private readonly ParameterBuilder _parameterBuilder = new();
    private readonly BlockBuilder _bodyBuilder = new();
    private ConstructorDeclarationSyntax _constructorDeclaration = SyntaxFactory.ConstructorDeclaration(parent.TypeName)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
    
    private bool _baseConstructorCalled = false;
    private List<string> _baseConstructorArguments = [ ];

    public ConstructorBuilder Parameter<T>(string parameterName)
    {
        _parameterBuilder.AddParameter<T>(parameterName);
        parent.AddRequirement<T>();
        
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

    public ConstructorBuilder MapBase()
    {
        _baseConstructorCalled = true;
        _baseConstructorArguments.Clear();
        
        return this;
    }

    public ConstructorBuilder MapBase(params string[] args)
    {
        _baseConstructorCalled = true;
        _baseConstructorArguments = args.ToList();
        
        return this;
    }

    public ConstructorDeclarationSyntax Build()
    {
        var parameterList = _parameterBuilder.Build();

        _constructorDeclaration = _constructorDeclaration.WithParameterList(parameterList);

        if (_baseConstructorCalled)
        {
            var arguments = _baseConstructorArguments.Any()
                ? _baseConstructorArguments.Select(arg => SyntaxFactory.Argument(SyntaxFactory.ParseExpression(arg)))
                : parameterList.Parameters.Select(param => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(param.Identifier)));
            var baseConstructorInitializer = SyntaxFactory.ConstructorInitializer(
                SyntaxKind.BaseConstructorInitializer,
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

            _constructorDeclaration = _constructorDeclaration.WithInitializer(baseConstructorInitializer);
        }

        _constructorDeclaration = _constructorDeclaration.WithBody(_bodyBuilder.Build());

        return _constructorDeclaration;
    }
}
