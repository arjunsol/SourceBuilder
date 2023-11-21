using G4ME.SourceBuilder.Syntax;

namespace G4ME.SourceBuilder.Types;

public class RecordBuilder(string recordName, string recordNamespace = "") : IRecordBuilder
{
    private readonly NamespaceCollection _requiredNamespaces = new(recordNamespace);
    private RecordDeclarationSyntax _recordDeclaration = SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName)
                                                            .AddModifiers(SyntaxFactory.Token(
                                                                SyntaxKind.PublicKeyword));
    public string ClassName { get; private set; } = recordName;
    public string Namespace { get; private set; } = recordNamespace;

    public RecordBuilder InheritsFrom<TBase>() where TBase : class
    {
        AddNamespace<TBase>();

        var baseTypeName = typeof(TBase).Name;
        _recordDeclaration = _recordDeclaration.AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName))
        );

        return this;
    }

    public RecordBuilder ImplementsInterface<TInterface>() where TInterface : class
    {
        AddNamespace<TInterface>();

        var interfaceName = typeof(TInterface).Name;
        _recordDeclaration = _recordDeclaration.AddBaseListTypes(
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(interfaceName)));

        return this;
    }

    public RecordBuilder WithProperty<T>(string propertyName, Action<PropertyBuilder> propertyConfigurator)
    {
        var propertyType = TypeName.ValueOf<T>();
        var propertyBuilder = new PropertyBuilder(propertyName, propertyType);
        propertyConfigurator(propertyBuilder);
        _recordDeclaration = _recordDeclaration.AddMembers(propertyBuilder.Build());

        AddNamespace<T>();

        return this;
    }

    public RecordBuilder WithMethod(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        var methodBuilder = new MethodBuilder(this, methodName);
        methodConfigurator(methodBuilder);
        _recordDeclaration = _recordDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public RecordBuilder WithMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        AddNamespace<T>();

        var methodBuilder = new MethodBuilder(this, typeof(T).Name, methodName);
        methodConfigurator(methodBuilder);
        _recordDeclaration = _recordDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public RecordBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator)
    {
        var attributeBuilder = new AttributeBuilder();
        attributeConfigurator(attributeBuilder);
        _recordDeclaration = _recordDeclaration.WithAttributeLists(attributeBuilder.Build());

        return this;
    }

    public void AddNamespace<T>() => _requiredNamespaces.Add<T>();

    public IEnumerable<string> GetRequiredNamespaces() => _requiredNamespaces.GetAll();

    public TypeDeclarationSyntax Build() => _recordDeclaration;
}
