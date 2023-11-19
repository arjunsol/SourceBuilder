using Microsoft.CodeAnalysis.CSharp;

namespace G4ME.SourceBuilder.Tests.Unit
{
    public class NamespaceBuilderTests
    {
        [Fact(Skip = "Nice to have")]
        public void FileScopedNamespace_ShouldIncludeNewLineAtEnd()
        {
            //var namespaceBuilder = new NamespaceBuilder();
            //var namespaceSyntax = namespaceBuilder.FileScoped("MyNamespace");

            //var trailingTrivia = namespaceSyntax.GetTrailingTrivia();

            //Assert.Single(trailingTrivia);
            //Assert.Equal(SyntaxKind.EndOfLineTrivia, trailingTrivia.First().Kind());
        }

        [Fact]
        public void StandardNamespace_ShouldNotIncludeNewLineAtEnd()
        {
            var namespaceBuilder = new NamespaceBuilder();
            var namespaceSyntax = namespaceBuilder.Standard("MyNamespace");

            var trailingTrivia = namespaceSyntax.GetTrailingTrivia();

            Assert.Empty(trailingTrivia);
        }
    }
}
