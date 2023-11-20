//namespace CodeGen;

//public class NamespaceBuilder
//{
//    private NamespaceDeclarationSyntax namespaceDeclaration;

//    public NamespaceBuilder(string name)
//    {
//        namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(name)).NormalizeWhitespace();
//    }

//    public ClassBuilder Add => new ClassBuilder("", "");

//    public NamespaceBuilder WithClass(ClassDeclarationSyntax classDeclaration)
//    {
//        namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);
//        return this;
//    }

//    public NamespaceDeclarationSyntax Build() => namespaceDeclaration;    
//}
