namespace G4ME.SourceBuilder.Types;

public class InterfaceBuilder(string name, string interfaceNamespace = "") : ITypeBuilder
{
    private readonly NamespaceCollection _requiredNamespaces = new(interfaceNamespace);
    private InterfaceDeclarationSyntax _interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(name)
                                                               .AddModifiers(SyntaxFactory.Token(
                                                                   SyntaxKind.PublicKeyword));

    public string TypeName { get; private set; } = name;
    public string Namespace { get; private set; } = interfaceNamespace;

    public InterfaceBuilder Extends<TBase>() where TBase : class
    {
        // TODO: Add to guard project
        if (!typeof(TBase).IsInterface)
        {
            throw new ArgumentException("Generic type must be an interface.", nameof(TBase));
        }

        AddNamespace<TBase>();
        var baseTypeName = typeof(TBase).Name;
        _interfaceDeclaration = _interfaceDeclaration.AddBaseListTypes(
                                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName)));
        return this;
    }

    public InterfaceBuilder AddMethod(string methodName, Action<InterfaceMethodBuilder> methodConfigurator)
    {
        InterfaceMethodBuilder methodBuilder = new(this, methodName);

        methodConfigurator(methodBuilder);
        _interfaceDeclaration = _interfaceDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public InterfaceBuilder AddMethod<T>(string methodName, Action<InterfaceMethodBuilder> methodConfigurator)
    {
        AddNamespace<T>();

        var returnType = Syntax.TypeName.ValueOf<T>();

        InterfaceMethodBuilder methodBuilder = new(this, returnType, methodName);
        methodConfigurator(methodBuilder);
        _interfaceDeclaration = _interfaceDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public InterfaceBuilder Properties(Action<PropertyBuilder> propertyConfigurator)
    {
        PropertyBuilder propertyBuilder = new();

        propertyConfigurator(propertyBuilder);
        _interfaceDeclaration = _interfaceDeclaration.AddMembers(propertyBuilder.Build());

        return this;
    }

    public InterfaceBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator)
    {
        AttributeBuilder attributeBuilder = new(this);
        attributeConfigurator(attributeBuilder);
        _interfaceDeclaration = _interfaceDeclaration.WithAttributeLists(attributeBuilder.Build());
        return this;
    }

    public IEnumerable<string> GetRequiredNamespaces() => _requiredNamespaces.GetAll();

    public TypeDeclarationSyntax Build() => _interfaceDeclaration.NormalizeWhitespace();

    public void AddNamespace<T>() => _requiredNamespaces.Add<T>();
}
