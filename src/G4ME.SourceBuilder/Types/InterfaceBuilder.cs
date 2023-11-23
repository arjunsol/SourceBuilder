namespace G4ME.SourceBuilder.Types;

public class InterfaceBuilder(string name, string interfaceNamespace = "") : IInterfaceBuilder
{
    private readonly NamespaceCollection _requiredNamespaces = new(interfaceNamespace);
    private InterfaceDeclarationSyntax _interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(name)
                                                                    .AddModifiers(SyntaxFactory.Token(
                                                                        SyntaxKind.PublicKeyword));

    public string ClassName { get; private set; } = name;

    public string Namespace => interfaceNamespace;

    public InterfaceBuilder InheritsFrom<TBase>() where TBase : class
    {
        AddNamespace<TBase>();

        var baseTypeName = typeof(TBase).Name;

        _interfaceDeclaration = _interfaceDeclaration.AddBaseListTypes(
                                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName)));

        return this;
    }

    public InterfaceBuilder WithMethod(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        var methodBuilder = new MethodBuilder(this, methodName);
        methodConfigurator(methodBuilder);
        _interfaceDeclaration = _interfaceDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public InterfaceBuilder Properties(Action<PropertyBuilder> propertyConfigurator)
    {
        var propertyBuilder = new PropertyBuilder();
        propertyConfigurator(propertyBuilder);
        _interfaceDeclaration = _interfaceDeclaration.AddMembers(propertyBuilder.Build());

        return this;
    }

    public InterfaceBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator)
    {
        var attributeBuilder = new AttributeBuilder();
        attributeConfigurator(attributeBuilder);
        _interfaceDeclaration = _interfaceDeclaration.WithAttributeLists(attributeBuilder.Build());

        return this;
    }

    public void AddNamespace<T>() => _requiredNamespaces.Add<T>();

    public IEnumerable<string> GetRequiredNamespaces() => _requiredNamespaces.GetAll();

    public TypeDeclarationSyntax Build() => _interfaceDeclaration;
}
