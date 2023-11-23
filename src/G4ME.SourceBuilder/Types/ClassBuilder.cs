namespace G4ME.SourceBuilder.Syntax;

public class ClassBuilder(string name, string classNamespace) : IClassBuilder
{
    private readonly NamespaceCollection _requiredNamespaces = new(classNamespace);
    private ClassDeclarationSyntax _classDeclaration = SyntaxFactory.ClassDeclaration(name)
                                                                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
    
    public string TypeName { get; private set; } = name;
    public string Namespace => classNamespace;

    public ClassBuilder(string name) : this(name, string.Empty) { }

    public ClassBuilder Extends<TBase>() where TBase : class
    {
        AddNamespace<TBase>();

        var baseTypeName = Syntax.TypeName.ValueOf<TBase>();
        _classDeclaration = _classDeclaration.AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName))
        );

        return this;
    }

    public ClassBuilder Implements<TInterface>() where TInterface : class
    {
        //TODO: Add to guard project
        if (!typeof(TInterface).IsInterface)
        {
            throw new ArgumentException("Generic type must be an interface.", nameof(TInterface));
        }


        AddNamespace<TInterface>();

        var interfaceName = Syntax.TypeName.ValueOf<TInterface>();
        _classDeclaration = _classDeclaration.AddBaseListTypes(
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(interfaceName)));

        return this;    
    }

    public ClassBuilder Attributes(Action<AttributeBuilder> attributeConfigurator)
    {
        AttributeBuilder attributeBuilder = new();

        attributeConfigurator(attributeBuilder);
        _classDeclaration = _classDeclaration.WithAttributeLists(attributeBuilder.Build());

        return this;
    }

    public ClassBuilder Properties(Action<PropertyBuilder> propertyConfigurator)
    {
        PropertyBuilder propertyBuilder = new();
        
        propertyConfigurator(propertyBuilder);
        _classDeclaration = _classDeclaration.AddMembers(propertyBuilder.Build());

        return this;
    }

    public ClassBuilder Constructor()
    {
        ConstructorBuilder constructorBuilder = new(this);
        _classDeclaration = _classDeclaration.AddMembers(constructorBuilder.Build());

        return this;
    }

    public ClassBuilder Constructor(Action<ConstructorBuilder> constructorConfigurator)
    {
        ConstructorBuilder constructorBuilder = new(this);
        constructorConfigurator(constructorBuilder);
        _classDeclaration = _classDeclaration.AddMembers(constructorBuilder.Build());

        return this;
    }

    public ClassBuilder AddMethod(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        MethodBuilder methodBuilder = new(this, methodName);

        methodConfigurator(methodBuilder);
        _classDeclaration = _classDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public ClassBuilder AddMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        AddNamespace<T>();
                
        string returnType = Syntax.TypeName.ValueOf<T>();

        MethodBuilder methodBuilder = new(this, returnType, methodName);
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
