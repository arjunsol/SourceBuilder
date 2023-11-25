namespace G4ME.SourceBuilder;

public class CompilationUnitBuilder(params ITypeBuilder[] typeBuilders)
{
    private readonly List<ITypeBuilder> _classBuilders = [.. typeBuilders];

    public CompilationUnitSyntax Build()
    {
        if (_classBuilders.Count == 0)
        {
            throw new InvalidOperationException("At least one type builder is required.");
        }

        // Get the namespace from the first class (assuming all classes share the same namespace)
        var namespaceName = _classBuilders.First().Namespace;
        HashSet<string> requiredNamespaces = [ ]; // TODO: Replace with namespace collection?

        // Aggregate required namespaces from all class builders
        foreach (var classBuilder in _classBuilders)
        {
            requiredNamespaces.UnionWith(classBuilder.GetRequiredNamespaces());
        }

        // Create the using directives
        var usingDirectives = requiredNamespaces.Select(ns => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns)))
                                               .ToArray();

        // Create the namespace and add class declarations //namespaceName
        FileScopedNamespaceDeclarationSyntax fileScopedNamespace = new NamespaceBuilder()
                                                                       .FileScoped(namespaceName);

        foreach (var classBuilder in _classBuilders)
        {
            TypeDeclarationSyntax classDeclaration = classBuilder.Build();
            
            if(_classBuilders.Count() > 1)
                classDeclaration = AddRegionDirectives(classBuilder, classDeclaration);

            // Add class declaration to the namespace declaration
            fileScopedNamespace = fileScopedNamespace.AddMembers(classDeclaration);
        }

        // Create the compilation unit and add the namespace declaration
        var compilationUnit = SyntaxFactory.CompilationUnit()
                                           .WithUsings(SyntaxFactory.List(usingDirectives))
                                           .AddMembers(fileScopedNamespace)
                                           .NormalizeWhitespace();

        return compilationUnit;
    }

    private static TypeDeclarationSyntax AddRegionDirectives(ITypeBuilder classBuilder, TypeDeclarationSyntax classDeclaration)
    {
        // Create #region directive
        var regionDirectiveStart = SyntaxFactory.Trivia(
            SyntaxFactory.RegionDirectiveTrivia(true)
            .WithEndOfDirectiveToken(
                SyntaxFactory.Token(SyntaxTriviaList.Create(SyntaxFactory.PreprocessingMessage($" {classBuilder.TypeName}")),
                SyntaxKind.EndOfDirectiveToken,
                SyntaxTriviaList.Create(SyntaxFactory.CarriageReturnLineFeed))));

        // Add region start trivia to the class declaration
        classDeclaration = classDeclaration.WithLeadingTrivia(regionDirectiveStart);

        // Create #endregion directive
        var regionDirectiveEnd = SyntaxFactory.Trivia(
            SyntaxFactory.EndRegionDirectiveTrivia(true)
            .WithEndOfDirectiveToken(
                SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken)
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)));

        // Add region end trivia to the class declaration
        classDeclaration = classDeclaration.WithTrailingTrivia(regionDirectiveEnd);

        return classDeclaration;
    }
}
