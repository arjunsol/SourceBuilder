namespace G4ME.SourceBuilder.Syntax;

public class NamespaceBuilder
{
    public FileScopedNamespaceDeclarationSyntax FileScoped(string namespaceName)
    {
        // Create file-scoped namespace
        var fileScopedNamespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(namespaceName))
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
            .NormalizeWhitespace();

        // Extract the namespace with the newline
        return fileScopedNamespace;
    }

    public NamespaceDeclarationSyntax Standard(string namespaceName)
    {
        // Create standard (braced) namespace
        var standardNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName))
            .NormalizeWhitespace();

        return standardNamespace;
    }
}
