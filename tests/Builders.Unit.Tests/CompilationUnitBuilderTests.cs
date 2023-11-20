

namespace CQRS.SourceGenerators.Test.CodeGen;

public class CompilationUnitBuilderTests
{
    [Fact]
    public void Build_WithSingleClass_ReturnsCompilationUnitWithOneClass()
    {
        // Arrange
        var classBuilder = new ClassBuilder("TestClass", "TestNamespace");
        var compilationUnitBuilder = new CompilationUnitBuilder(classBuilder);

        // Act
        var compilationUnit = compilationUnitBuilder.Build();
        var declaredClasses = compilationUnit.DescendantNodes()
                                             .OfType<ClassDeclarationSyntax>();

        // Assert
        Assert.Single(declaredClasses);
        Assert.Equal("TestClass", declaredClasses.First().Identifier.Text);
    }

    [Fact]
    public void Build_WithMultipleClasses_ReturnsCompilationUnitWithMultipleClasses()
    {
        // Arrange
        var classBuilder1 = new ClassBuilder("TestClass1", "TestNamespace");
        var classBuilder2 = new ClassBuilder("TestClass2", "TestNamespace");
        var compilationUnitBuilder = new CompilationUnitBuilder(classBuilder1, classBuilder2);

        // Act
        var compilationUnit = compilationUnitBuilder.Build();
        var declaredClasses = compilationUnit.DescendantNodes()
                                             .OfType<ClassDeclarationSyntax>()
                                             .ToList();

        // Assert
        Assert.Equal(2, declaredClasses.Count);
        Assert.Contains(declaredClasses, c => c.Identifier.Text == "TestClass1");
        Assert.Contains(declaredClasses, c => c.Identifier.Text == "TestClass2");
    }

    [Fact]
    public void Build_WithClassNamespace_CreatesCorrectNamespace()
    {
        // Arrange
        var classBuilder = new ClassBuilder("TestClass", "TestNamespace");
        var compilationUnitBuilder = new CompilationUnitBuilder(classBuilder);

        // Act
        var compilationUnit = compilationUnitBuilder.Build();
        var declaredNamespace = compilationUnit.DescendantNodes()
                                               .OfType<NamespaceDeclarationSyntax>()
                                               .FirstOrDefault();

        // Assert
        Assert.NotNull(declaredNamespace);
        Assert.Equal("TestNamespace", declaredNamespace.Name.ToString());
    }

    [Fact]
    public void Build_WithRequiredNamespaces_IncludesUsings()
    {
        // Arrange
        var classBuilder = new ClassBuilder("TestClass", "TestNamespace");
        // Assuming ClassBuilder.AddNamespace method adds a namespace to requiredNamespaces
        classBuilder.AddNamespace(typeof(System.Text.StringBuilder));
        var compilationUnitBuilder = new CompilationUnitBuilder(classBuilder);

        // Act
        var compilationUnit = compilationUnitBuilder.Build();
        var usingDirectives = compilationUnit.DescendantNodes()
                                             .OfType<UsingDirectiveSyntax>()
                                             .ToList();

        // Assert
        Assert.Contains(usingDirectives, u =>
        {
            Assert.NotNull(u.Name);
            return u.Name.ToString() == "System.Text";
        });
    }

    [Fact]
    public void Build_WithNoClassBuilders_ThrowsInvalidOperationException()
    {
        // Arrange
        var compilationUnitBuilder = new CompilationUnitBuilder();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => compilationUnitBuilder.Build());
    }
}
