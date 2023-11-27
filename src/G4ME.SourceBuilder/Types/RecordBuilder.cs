
using G4ME.SourceBuilder.Compile;

namespace G4ME.SourceBuilder.Syntax;

public class RecordBuilder(string recordName, string recordNamespace = "") : ITypeBuilder
{
    private readonly Requirements _requirements = new(recordNamespace);
    private readonly ParameterBuilder _parameterBuilder = new();

    private RecordDeclarationSyntax _recordDeclaration = SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName)
                                                                       .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                                                       .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    
    public string TypeName { get; private set; } = recordName;
    public string Namespace => recordNamespace;

    public RecordBuilder(string name) : this(name, string.Empty) { }

    public RecordBuilder Extends<TBase>() where TBase : class
    {
        // TODO: Add to guard project (reliable record detection)
        //if (!typeof(TBase).IsValueType) 
        //{ 
        //    throw new ArgumentException("Generic type must be a record.", nameof(TBase));
        //}
        
        AddRequirement<TBase>();

        var baseTypeName = typeof(TBase).Name;
        _recordDeclaration = _recordDeclaration.AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName))
        );

        return this;
    }

    public RecordBuilder Implements<TInterface>() where TInterface : class
    {
        if (!typeof(TInterface).IsInterface)
        {
            throw new ArgumentException("Generic type must be an interface.", nameof(TInterface));
        }

        AddRequirement<TInterface>();

        var interfaceName = Syntax.TypeName.ValueOf<TInterface>();
        _recordDeclaration = _recordDeclaration.AddBaseListTypes(
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(interfaceName)));

        return this;
    }

    public RecordBuilder Attributes(Action<AttributeBuilder> attributeConfigurator)
    {
        AttributeBuilder attributeBuilder = new(this);

        attributeConfigurator(attributeBuilder);
        _recordDeclaration = _recordDeclaration.WithAttributeLists(attributeBuilder.Build());

        return this;
    }

    // TODO: All TypeBuilder should follow a configurable ruleset on naming standards
    public RecordBuilder Parameter<T>(string parameterName)
    {
        _parameterBuilder.AddParameter<T>(parameterName);

        AddRequirement<T>();

        return this;
    }

    //TODO: Find a way to support properties
    //public RecordBuilder Properties(Action<PropertyBuilder> propertyConfigurator)
    //{
    //    PropertyBuilder propertyBuilder = new();
        
    //    propertyConfigurator(propertyBuilder);
    //    _recordDeclaration = _recordDeclaration.AddMembers(propertyBuilder.Build());

    //    return this;
    //}
    
    //TODO: Find a way to support methods
    //public RecordBuilder AddMethod(string methodName, Action<MethodBuilder> methodConfigurator)
    //{
    //    MethodBuilder methodBuilder = new(this, methodName);
        
    //    methodConfigurator(methodBuilder);
    //    _recordDeclaration = _recordDeclaration.AddMembers(methodBuilder.Build());

    //    return this;
    //}

    //public RecordBuilder AddMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator)
    //{
    //    AddNamespace<T>();

    //    string returnType = Syntax.TypeName.ValueOf<T>();

    //    var methodBuilder = new MethodBuilder(this, returnType, methodName);
    //    methodConfigurator(methodBuilder);
    //    _recordDeclaration = _recordDeclaration.AddMembers(methodBuilder.Build());

    //    return this;
    //}

    public void AddRequirement<T>() => _requirements.Add<T>();

    public TypeDeclarationSyntax Build()
    {
        _recordDeclaration = _recordDeclaration
                             .WithParameterList(_parameterBuilder.Build());

        return _recordDeclaration;
    }

    public Requirements GetRequirements() => _requirements;
}
