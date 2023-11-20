namespace G4ME.SourceBuilder;

public class ClassBuilder(string name, string classNamespace) : ITypeBuilder, IClassBuilder
{
    private readonly HashSet<string> _requiredNamespaces = [];
    private readonly string _classNamespace = classNamespace;

    private ClassDeclarationSyntax _classDeclaration = SyntaxFactory.ClassDeclaration(name)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

    public string ClassName { get; private set; } = name;
    public string Namespace => _classNamespace;

    public ClassBuilder(string name) : this(name, "") { }

    public ClassBuilder InheritsFrom<TBase>() where TBase : class
    {
        AddNamespace(typeof(TBase));
        var baseTypeName = typeof(TBase).Name;
        _classDeclaration = _classDeclaration.AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName))
        );
        return this;
    }

    public ClassBuilder ImplementsInterface<TInterface>() where TInterface : class
    {
        AddNamespace(typeof(TInterface));
        var interfaceName = typeof(TInterface).Name;
        _classDeclaration = _classDeclaration.AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(interfaceName))
        );
        return this;
    }

    public ClassBuilder WithProperty<T>(string propertyName, Action<PropertyBuilder> propertyConfigurator)
    {
        var propertyType = typeof(T).Name;
        var propertyBuilder = new PropertyBuilder(propertyName, propertyType);
        propertyConfigurator(propertyBuilder);
        _classDeclaration = _classDeclaration.AddMembers(propertyBuilder.Build());
        AddNamespace(typeof(T));
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
        var methodBuilder = new MethodBuilder(this, typeof(T).Name, methodName);
        methodConfigurator(methodBuilder);
        _classDeclaration = _classDeclaration.AddMembers(methodBuilder.Build());
        AddNamespace(typeof(T));
        return this;
    }

    public ClassBuilder WithMethod(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        var methodBuilder = new MethodBuilder(this, methodName);
        methodConfigurator(methodBuilder);
        _classDeclaration = _classDeclaration.AddMembers(methodBuilder.Build());
        return this;
    }



    //public ClassBuilder WithIndexer(IndexerDeclarationSyntax indexerDeclaration)
    //{
    //    _classDeclaration = _classDeclaration.AddMembers(indexerDeclaration);
    //    return this;
    //}

    public void AddNamespace(Type type)
    {
        if (InvalidNamespace(type)) return;

        ArgumentNullException.ThrowIfNull(type.Namespace);

        _requiredNamespaces.Add(type.Namespace);

        if (type.IsGenericType)
        {
            foreach (var argument in type.GetGenericArguments())
            {
                AddNamespace(argument);
            }
        }
    }

    private bool InvalidNamespace(Type type) => IsPrimitive(type) ||
                                                type.Namespace is null ||
                                                type.Namespace == _classNamespace ||
                                                _requiredNamespaces.Contains(type.Namespace);

    private static bool IsPrimitive(Type type) => type.IsPrimitive ||
                                           type.Equals(typeof(string)) ||
                                           type.Equals(typeof(decimal));

    public IEnumerable<string> GetRequiredNamespaces() => _requiredNamespaces;

    public TypeDeclarationSyntax Build() => _classDeclaration;
}
