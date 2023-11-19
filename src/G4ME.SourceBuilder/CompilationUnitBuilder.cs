
using G4ME.SourceBuilder.Compile;

namespace G4ME.SourceBuilder;

public class CompilationUnitBuilder(params ITypeBuilder<TypeDeclarationSyntax>[] typeBuilders)
{
    private readonly List<ITypeBuilder<TypeDeclarationSyntax>> _typeBuilders = [.. typeBuilders];
    private readonly Requirements _requirements = new(string.Empty);

    public CompilationUnitSyntax Build()
    {
        if (_typeBuilders.Count == 0)
        {
            throw new InvalidOperationException("At least one type builder is required.");
        }

        // Get the namespace from the first class (assuming all classes share the same namespace)
        string namespaceName = _typeBuilders.First().Namespace;
        
        // Aggregate required namespaces from all class builders
        foreach (ITypeBuilder<TypeDeclarationSyntax> typeBuilder in _typeBuilders)
        {
            _requirements.UnionWith(typeBuilder.GetRequirements());
        }

        // Create the using directives
        UsingDirectiveSyntax[] usingDirectives = _requirements.Namespaces.Select(ns => 
                                                               SyntaxFactory.UsingDirective(
                                                                   SyntaxFactory.ParseName(ns)))
                                                                       .ToArray();

        // Create the namespace and add class declarations //namespaceName
        FileScopedNamespaceDeclarationSyntax fileScopedNamespace = new NamespaceBuilder()
                                                                       .FileScoped(namespaceName);

        foreach (ITypeBuilder<TypeDeclarationSyntax> typeBuilder in _typeBuilders)
        {
            TypeDeclarationSyntax classDeclaration = typeBuilder.Build();
            
            if(_typeBuilders.Count() > 1)
                classDeclaration = AddRegionDirectives(typeBuilder, classDeclaration);

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

    private static TypeDeclarationSyntax AddRegionDirectives(ITypeBuilder<TypeDeclarationSyntax> classBuilder, TypeDeclarationSyntax classDeclaration)
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

    public IEnumerable<MetadataReference> GetRequiredReferences()
    {
        _requirements.References.AddDotNetReferences();
        return _requirements.References;
    }
}
