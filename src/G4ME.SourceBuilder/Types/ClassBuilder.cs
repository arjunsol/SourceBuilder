using G4ME.SourceBuilder.Compile;

namespace G4ME.SourceBuilder.Syntax;

// ClassBuilder: Used for building a class structure with specified characteristics.
public class ClassBuilder(string name, string classNamespace) : IClassBuilder
{
    // Private fields for managing constraints and requirements.
    private readonly Requirements _requirements = new(classNamespace);

    // The core syntax for declaring a class.
    private ClassDeclarationSyntax _classDeclaration = SyntaxFactory.ClassDeclaration(name)
                                                                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

    // Properties for accessing class name and namespace.
    public string TypeName { get; private set; } = name;
    public string Namespace => classNamespace;

    // Constructor for the class with optional namespace.
    public ClassBuilder(string name) : this(name, string.Empty) { }

    // Method for configuring attributes of the class.
    public IClassAttributesConfigured Attributes(Action<AttributeBuilder> attributeConfigurator)
    {
        AttributeBuilder attributeBuilder = new(this);
        attributeConfigurator(attributeBuilder);
        _classDeclaration = _classDeclaration.WithAttributeLists(attributeBuilder.Build());
        return this;
    }

    // Method for setting a base class from which the current class extends.
    public IClassBaseConfigured Extends<TBase>() where TBase : class
    {
        //TODO: Guard clause to ensure a class doesn't extend multiple base classes.
        
        AddRequirement<TBase>();
        var baseTypeName = Syntax.TypeName.ValueOf<TBase>();
        _classDeclaration = _classDeclaration.AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName))
        );

        return this;
    }

    // Method for implementing interfaces in the class.
    public IClassInterfacesConfigured Implements<TInterface>() where TInterface : class
    {
        // Validation to ensure TInterface is indeed an interface. //TODO: Check for multiple interfaces of same type.
        if (!typeof(TInterface).IsInterface)
        {
            throw new ArgumentException("Generic type must be an interface.", nameof(TInterface));
        }

        AddRequirement<TInterface>();
        Type interfaceType = typeof(TInterface);

        // Handling generic interface names.
        string interfaceName = interfaceType.IsGenericType
                            ? GetGenericTypeName(interfaceType)
                            : interfaceType.Name;

        _classDeclaration = _classDeclaration.AddBaseListTypes(
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(interfaceName)));

        return this;
    }

    // Method to add properties to the class.
    public IClassPropertiesConfigured Properties(Action<PropertyBuilder> propertyConfigurator)
    {
        PropertyBuilder propertyBuilder = new();
        propertyConfigurator(propertyBuilder);
        _classDeclaration = _classDeclaration.AddMembers(propertyBuilder.Build());

        return this;
    }

    // Method to add constructors to the class. Includes overloads.
    public IClassBuilderAddMethods Constructor()
    {
        ConstructorBuilder constructorBuilder = new(this);
        ConstructorDeclarationSyntax constructor = constructorBuilder.Build();
        AddConstructor(constructor);

        return this;
    }

    // Overload to provide configuration action for the constructor.
    public IClassBuilderAddMethods Constructor(Action<ConstructorBuilder> constructorConfigurator)
    {
        ConstructorBuilder constructorBuilder = new(this);
        constructorConfigurator(constructorBuilder);
        ConstructorDeclarationSyntax constructor = constructorBuilder.Build();
        AddConstructor(constructor);

        return this;
    }

    // Method to add methods to the class. Includes overloads for return types.
    public IClassBuilderAddMethods AddMethod(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        MethodBuilder methodBuilder = new(this, methodName);
        methodConfigurator(methodBuilder);
        _classDeclaration = _classDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    // Overload for methods with specified return types.
    public IClassBuilderAddMethods AddMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        AddRequirement<T>();
        string returnType = Syntax.TypeName.ValueOf<T>();

        MethodBuilder methodBuilder = new(this, returnType, methodName);
        methodConfigurator(methodBuilder);
        _classDeclaration = _classDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    // Method to add generic requirements.
    public void AddRequirement<T>() => _requirements.Add<T>();

    // Method to build and get the final class declaration syntax.
    public ClassDeclarationSyntax Build() => _classDeclaration;

    // Method to retrieve current requirements.
    public Requirements GetRequirements() => _requirements;

    #region private methods
    // Private method to handle the addition of constructors.
    private void AddConstructor(ConstructorDeclarationSyntax constructor)
    {
        IEnumerable<MemberDeclarationSyntax> constructors = _classDeclaration.Members.Where(m => m.GetType().IsAssignableFrom(typeof(ConstructorDeclarationSyntax)));

        // Validation to avoid duplicate constructors.
        if (ConstructorsMatch(constructors, constructor))
        {
            throw new InvalidOperationException("Class already contains a constructor with the same parameters.");
        }

        _classDeclaration = _classDeclaration.AddMembers(constructor);
    }

    // Private method to check if constructors match.
    private bool ConstructorsMatch(IEnumerable<MemberDeclarationSyntax> constructors, ConstructorDeclarationSyntax constructor)
    {
        foreach (ConstructorDeclarationSyntax existingConstructor in constructors)
        {
            if (ParametersMatch(existingConstructor.ParameterList, constructor.ParameterList))
            {
                return true;
            }
        }

        return false;
    }

    // Private method to compare parameters of two constructors for matching.
    private bool ParametersMatch(ParameterListSyntax existingParameters, ParameterListSyntax newParameters)
    {
        // First, check if the number of parameters in both lists is the same.
        // If the count is different, the parameter lists do not match.
        if (existingParameters.Parameters.Count != newParameters.Parameters.Count)
        {
            return false;
        }

        // Flag to keep track of whether the parameters match.
        bool parametersMatch = true;

        // Loop through each parameter in the lists to compare them.
        for (int i = 0; i < existingParameters.Parameters.Count; i++)
        {
            // Retrieve the types of the current parameters in both lists.
            var existingType = existingParameters.Parameters[i].Type;
            var newType = newParameters.Parameters[i].Type;

            // If either of the parameter types is null, or if they don't match,
            // set the flag to false and break out of the loop.
            if (existingType is null || newType is null || existingType.ToString() != newType.ToString())
            {
                parametersMatch = false;
                break;
            }
        }

        // Return the final result indicating whether the parameters match.
        return parametersMatch;
    }


    // Helper method to format the name of generic types.
    private string GetGenericTypeName(Type type)
    {
        Type[] genericArguments = type.GetGenericArguments();
        string typeNameWithoutArity = type.Name.Split('`')[0];
        string formattedGenericArguments = string.Join(", ", genericArguments.Select(arg => arg.Name));

        return $"{typeNameWithoutArity}<{formattedGenericArguments}>";
    }
    #endregion
}
