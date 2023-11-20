namespace G4ME.SourceBuilder;

public class CompilationUnitBuilder(params ITypeBuilder[] classBuilders)
{
    private readonly List<ITypeBuilder> _classBuilders = [.. classBuilders];

    public CompilationUnitSyntax Build()
    {
        if (_classBuilders.Count == 0)
        {
            throw new InvalidOperationException("At least one class builder is required.");
        }

        // Get the namespace from the first class (assuming all classes share the same namespace)
        var namespaceName = _classBuilders.First().Namespace;
        var requiredNamespaces = new HashSet<string>();

        // Aggregate required namespaces from all class builders
        foreach (var classBuilder in _classBuilders)
        {
            requiredNamespaces.UnionWith(classBuilder.GetRequiredNamespaces());
        }

        // Create the using directives
        var usingDirectives = requiredNamespaces.Select(ns => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns)))
                                               .ToArray();

        // Create the namespace and add class declarations
        var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName))
                                                .WithUsings(SyntaxFactory.List(usingDirectives));

        foreach (var classBuilder in _classBuilders)
        {
            var classDeclaration = classBuilder.Build();

            // Creating a region around each class
            var regionTriviaStart = SyntaxFactory.Trivia(SyntaxFactory.RegionDirectiveTrivia(true)
                .WithEndOfDirectiveToken(SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken)));
            var regionTriviaEnd = SyntaxFactory.Trivia(SyntaxFactory.EndRegionDirectiveTrivia(true));

            var leadingTrivia = SyntaxFactory.TriviaList(regionTriviaStart)
                .Add(SyntaxFactory.Comment($"#region {classBuilder.ClassName}"));
            var trailingTrivia = SyntaxFactory.TriviaList(regionTriviaEnd);

            classDeclaration = classDeclaration
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia(trailingTrivia);

            namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);
        }

        // Create the compilation unit and add the namespace declaration
        var compilationUnit = SyntaxFactory.CompilationUnit()
                                            .AddMembers(namespaceDeclaration)
                                            .NormalizeWhitespace();

        return compilationUnit;
    }
}
