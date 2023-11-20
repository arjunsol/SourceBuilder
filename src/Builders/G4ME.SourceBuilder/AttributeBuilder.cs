namespace G4ME.SourceBuilder;

public class AttributeBuilder
{
    private readonly List<AttributeSyntax> _attributes = [];

    public AttributeBuilder AddAttribute<TAttribute>(params string[] arguments) where TAttribute : Attribute
    {
        var attributeName = typeof(TAttribute).Name;
        if (attributeName.EndsWith("Attribute"))
        {
            attributeName = attributeName[0..^9]; // Remove "Attribute" suffix
        }

        var argumentList = SyntaxFactory.AttributeArgumentList(
            SyntaxFactory.SeparatedList(arguments.Select(arg =>
                SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression(arg)))));

        var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(attributeName), argumentList);
        _attributes.Add(attribute);
        return this;
    }

    public SyntaxList<AttributeListSyntax> Build()
    {
        return SyntaxFactory.List(_attributes.Select(attr =>
            SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attr))));
    }
}
