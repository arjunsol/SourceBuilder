namespace G4ME.SourceBuilder.Types;

public class ClassBuilder(string name, string classNamespace) : IClassBuilder
{
    private readonly string _classNamespace = classNamespace;
    private readonly NamespaceCollection _requiredNamespaces = new(classNamespace);

    private ClassDeclarationSyntax _classDeclaration = SyntaxFactory.ClassDeclaration(name)
                                                            .AddModifiers(SyntaxFactory.Token(
                                                                SyntaxKind.PublicKeyword));

    public string ClassName { get; private set; } = name;

    public string Namespace => _classNamespace;

    public ClassBuilder(string name) : this(name, string.Empty) { }

    public ClassBuilder InheritsFrom<TBase>() where TBase : class
    {
        AddNamespace<TBase>();

        var baseTypeName = typeof(TBase).Name;
        _classDeclaration = _classDeclaration.AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName))
        );

        return this;
    }

    public ClassBuilder ImplementsInterface<TInterface>() where TInterface : class
    {
        AddNamespace<TInterface>();

        var interfaceName = TypeName.ValueOf<TInterface>();
        _classDeclaration = _classDeclaration.AddBaseListTypes(
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(interfaceName)));

        return this;
    }

    public ClassBuilder WithProperty<T>(string propertyName, Action<PropertyBuilder> propertyConfigurator)
    {
        AddNamespace<T>();

        var propertyType = TypeName.ValueOf<T>();
        var propertyBuilder = new PropertyBuilder(propertyName, propertyType);
        propertyConfigurator(propertyBuilder);
        _classDeclaration = _classDeclaration.AddMembers(propertyBuilder.Build());

        return this;
    }

    public ClassBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator)
    {
        var attributeBuilder = new AttributeBuilder();
        attributeConfigurator(attributeBuilder);
        _classDeclaration = _classDeclaration.WithAttributeLists(attributeBuilder.Build());

        return this;
    }

    public ClassBuilder WithConstructor(Action<ConstructorBuilder> constructorConfigurator)
    {
        var constructorBuilder = new ConstructorBuilder(this);
        constructorConfigurator(constructorBuilder);
        _classDeclaration = _classDeclaration.AddMembers(constructorBuilder.Build());

        return this;
    }

    public ClassBuilder WithMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        AddNamespace<T>();
                
        string returnType = TypeName.ValueOf<T>();

        var methodBuilder = new MethodBuilder(this, returnType, methodName);
        methodConfigurator(methodBuilder);
        _classDeclaration = _classDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public ClassBuilder WithMethod(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        var methodBuilder = new MethodBuilder(this, methodName);
        methodConfigurator(methodBuilder);
        _classDeclaration = _classDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public void AddNamespace<T>() => _requiredNamespaces.Add<T>();

    //public ClassBuilder WithIndexer(IndexerDeclarationSyntax indexerDeclaration)
    //{
    //    _classDeclaration = _classDeclaration.AddMembers(indexerDeclaration);
    //    return this;
    //}

    public IEnumerable<string> GetRequiredNamespaces() => _requiredNamespaces.GetAll();

    public TypeDeclarationSyntax Build() => _classDeclaration;
}
