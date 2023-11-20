namespace G4ME.SourceBuilder;

public class PropertyBuilder(string propertyName, string propertyType)
{
    private PropertyDeclarationSyntax _propertyDeclaration = SyntaxFactory.PropertyDeclaration(
            SyntaxFactory.ParseTypeName(propertyType), propertyName)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

    public PropertyBuilder WithGetter()
    {
        var getter = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        _propertyDeclaration = _propertyDeclaration.AddAccessorListAccessors(getter);
        return this;
    }

    public PropertyBuilder WithSetter()
    {
        var setter = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        _propertyDeclaration = _propertyDeclaration.AddAccessorListAccessors(setter);
        return this;
    }

    public PropertyBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator)
    {
        var attributeBuilder = new AttributeBuilder();
        attributeConfigurator(attributeBuilder);
        _propertyDeclaration = _propertyDeclaration.WithAttributeLists(attributeBuilder.Build());
        return this;
    }

    public PropertyDeclarationSyntax Build() => _propertyDeclaration;
    
}
