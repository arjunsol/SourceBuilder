using G4ME.SourceBuilder.Compile;

namespace G4ME.SourceBuilder.Syntax;

// RecordBuilder: A builder class for constructing C# record types dynamically.
public class RecordBuilder(string recordName, string recordNamespace = "") : IRecordBuilder
{
    // Fields for tracking requirements and parameter configurations.
    private readonly Requirements _requirements = new(recordNamespace);
    private readonly ParameterBuilder _parameterBuilder = new();

    // The core syntax for declaring a record.
    private RecordDeclarationSyntax _recordDeclaration = SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName)
                                                                      .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

    // Properties for accessing the record's name and namespace.
    public string TypeName { get; private set; } = recordName;
    public string Namespace => recordNamespace;

    // Constructor for the record with a name and optional namespace.
    public RecordBuilder(string name) : this(name, string.Empty) { }

    // Method for specifying a base class from which the record will extend.
    public IRecordBaseConfigured Extends<TBase>() where TBase : class
    {
        // TODO: Validate that TBase is a valid record type.
        // Add the base type requirement.
        AddRequirement<TBase>();

        // Determine the base type's name and add it to the record declaration.
        var baseTypeName = typeof(TBase).Name;
        _recordDeclaration = _recordDeclaration.AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName))
        );

        return this;
    }

    // Method for implementing interfaces in the record.
    public IRecordBaseConfigured Implements<TInterface>() where TInterface : class
    {
        // Ensure the generic type is an interface.
        if (!typeof(TInterface).IsInterface)
        {
            throw new ArgumentException("Generic type must be an interface.", nameof(TInterface));
        }

        // Add the interface to the requirements and update the record declaration.
        AddRequirement<TInterface>();
        Type interfaceType = typeof(TInterface);
        string interfaceName = interfaceType.IsGenericType
                            ? GetGenericTypeName(interfaceType)
                            : interfaceType.Name;
        _recordDeclaration = _recordDeclaration.AddBaseListTypes(
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(interfaceName)));

        return this;
    }

    // Method to configure attributes for the record.
    public IRecordAttributesConfigured Attributes(Action<AttributeBuilder> attributeConfigurator)
    {
        AttributeBuilder attributeBuilder = new(this);
        attributeConfigurator(attributeBuilder);
        _recordDeclaration = _recordDeclaration.WithAttributeLists(attributeBuilder.Build());

        return this;
    }

    // Method for adding parameters to the record.
    public IRecordAttributesConfigured Parameter<T>(string parameterName)
    {
        // Add parameter to the parameter builder and update requirements.
        _parameterBuilder.AddParameter<T>(parameterName);
        AddRequirement<T>();

        return this;
    }

    // Method for adding necessary requirements for the record.
    public void AddRequirement<T>() => _requirements.Add<T>();

    // Method to build the final record declaration syntax.
    public RecordDeclarationSyntax Build()
    {
        // Combine all configured features into the record declaration.
        _recordDeclaration = _recordDeclaration.WithParameterList(_parameterBuilder.Build());
        return _recordDeclaration;
    }

    // Method to get the current requirements of the record.
    public Requirements GetRequirements() => _requirements;

    // Helper method to format the name of generic types.
    private string GetGenericTypeName(Type type)
    {
        // Extract and format generic type arguments.
        Type[] genericArguments = type.GetGenericArguments();
        string typeNameWithoutArity = type.Name.Split('`')[0];
        string formattedGenericArguments = string.Join(", ", genericArguments.Select(arg => arg.Name));
        return $"{typeNameWithoutArity}<{formattedGenericArguments}>";
    }
}
