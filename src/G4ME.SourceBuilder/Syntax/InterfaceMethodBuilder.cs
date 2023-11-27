using G4ME.SourceBuilder.Types;

namespace G4ME.SourceBuilder.Syntax;

public class InterfaceMethodBuilder(ITypeBuilder parent, string returnType, string methodName)
{
    private readonly ParameterBuilder _parameterBuilder = new();

    private MethodDeclarationSyntax _methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnType), methodName);

    public InterfaceMethodBuilder(InterfaceBuilder parent, string methodName) : this(parent, "void", methodName) { }

    public InterfaceMethodBuilder Parameter<T>(string parameterName)
    {
        _parameterBuilder.AddParameter<T>(parameterName);
        
        parent.AddRequirement<T>();

        return this;
    }

    public InterfaceMethodBuilder Attributes(Action<AttributeBuilder> attributeConfigurator)
    {
        AttributeBuilder attributeBuilder = new(parent);

        attributeConfigurator(attributeBuilder);
        _methodDeclaration = _methodDeclaration.WithAttributeLists(attributeBuilder.Build());
        
        return this;
    }

    public MethodDeclarationSyntax Build()
    {
        // Add a semicolon to the method declaration syntax
        return _methodDeclaration
            .WithParameterList(_parameterBuilder.Build())
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }

}
