namespace G4ME.SourceBuilder.Syntax;

public class PropertyBuilder
{
    private const SyntaxKind DEFAULT_MODIFIER = SyntaxKind.PublicKeyword;
    private readonly PropertyCollection _properites = new();

    public PropertyBuilder Add<T>(string propertyName) => Add<T>(propertyName, DEFAULT_MODIFIER);

    public PropertyBuilder AddPrivate<T>(string propertyName) => Add<T>(propertyName, SyntaxKind.PrivateKeyword);

    public PropertyBuilder Get()
    {
        var getter = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        AddAccessor(getter);

        return this;
    }

    public PropertyBuilder Set()
    {
        var setter = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        AddAccessor(setter);

        return this;
    }

    public PropertyBuilder PrivateSet()
    {
        var setter = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        AddAccessor(setter);

        return this;
    }

    public PropertyDeclarationSyntax[] Build() => _properites.Build();

    private PropertyBuilder Add<T>(string propertyName, SyntaxKind modifier)
    {
        string typeName = TypeName.ValueOf<T>();

        var property = SyntaxFactory.PropertyDeclaration(
            SyntaxFactory.ParseTypeName(typeName),
            SyntaxFactory.Identifier(propertyName))
            .AddModifiers(SyntaxFactory.Token(modifier));

        _properites.AddProperty(property);

        return this;
    }

    private void AddAccessor(AccessorDeclarationSyntax accessor)
    {
        var currentProperty = _properites.CurrentProperty;
        var updatedProperty = currentProperty.AddAccessorListAccessors(accessor);
        _properites.UpdateCurrentProperty(updatedProperty);
    }
}
