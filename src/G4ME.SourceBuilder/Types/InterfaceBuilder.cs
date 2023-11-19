using System.Xml.Linq;
using G4ME.SourceBuilder.Compile;

namespace G4ME.SourceBuilder.Types;

public class InterfaceBuilder(string name, string interfaceNamespace = "") : IInterfaceBuilder
{
    // Requirements for the interface, including namespaces and other dependencies.
    private readonly Requirements _requirements = new(interfaceNamespace);

    // Declaration syntax for the interface, initially set with the name and public modifier.
    private InterfaceDeclarationSyntax _interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(name)
                                                               .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

    // Properties for accessing the interface's name and namespace.
    public string TypeName { get; private set; } = name;
    public string Namespace { get; private set; } = interfaceNamespace;

    // Method for specifying base interfaces that this interface extends.
    public IInterfaceExtended Extends<TBase>() where TBase : class
    {
        // Guard clause to ensure the base type is also an interface.
        if (!typeof(TBase).IsInterface)
        {
            throw new ArgumentException("Generic type must be an interface.", nameof(TBase));
        }

        // Add the base type to requirements and update the interface declaration.
        AddRequirement<TBase>();
        var baseTypeName = typeof(TBase).Name;
        _interfaceDeclaration = _interfaceDeclaration.AddBaseListTypes(
                                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName)));
        return this;
    }

    // Method for adding a method to the interface.
    public IInterfaceAddMethods AddMethod(string methodName, Action<InterfaceMethodBuilder> methodConfigurator)
    {
        // Create a new method builder and apply the configurator.
        InterfaceMethodBuilder methodBuilder = new(this, methodName);
        methodConfigurator(methodBuilder);

        // Add the built method to the interface declaration.
        _interfaceDeclaration = _interfaceDeclaration.AddMembers(methodBuilder.Build());
        return this;
    }

    // Overloaded method for adding a method with a return type.
    public IInterfaceAddMethods AddMethod<T>(string methodName, Action<InterfaceMethodBuilder> methodConfigurator)
    {
        // Add the return type to the requirements.
        AddRequirement<T>();

        var returnType = Syntax.TypeName.ValueOf<T>();
        InterfaceMethodBuilder methodBuilder = new(this, returnType, methodName);
        methodConfigurator(methodBuilder);

        // Add the built method to the interface declaration.
        _interfaceDeclaration = _interfaceDeclaration.AddMembers(methodBuilder.Build());
        return this;
    }

    // Method for adding properties to the interface.
    public IInterfaceAddMethods Properties(Action<PropertyBuilder> propertyConfigurator)
    {
        // Create a new property builder and apply the configurator.
        PropertyBuilder propertyBuilder = new();
        propertyConfigurator(propertyBuilder);

        // Add the built properties to the interface declaration.
        _interfaceDeclaration = _interfaceDeclaration.AddMembers(propertyBuilder.Build());
        return this;
    }

    // Method for adding attributes to the interface.
    public IInterfaceAttributesAdded Attributes(Action<AttributeBuilder> attributeConfigurator)
    {
        // Create a new attribute builder and apply the configurator.
        AttributeBuilder attributeBuilder = new(this);
        attributeConfigurator(attributeBuilder);

        // Add the built attributes to the interface declaration.
        _interfaceDeclaration = _interfaceDeclaration.WithAttributeLists(attributeBuilder.Build());
        return this;
    }

    // Method to build and return the final interface declaration syntax.
    public InterfaceDeclarationSyntax Build() => _interfaceDeclaration.NormalizeWhitespace();

    // Method to add a generic requirement.
    public void AddRequirement<T>() => _requirements.Add<T>();

    // Method to retrieve the current requirements for the interface.
    public Requirements GetRequirements() => _requirements;
}
