namespace G4ME.SourceBuilder.Syntax;

/// <summary>
/// Provides the base functionality for building a class with specified attributes.
/// </summary>
public interface IClassBuilder : IClassAttributesConfigured, ITypeBuilder<ClassDeclarationSyntax>
{
    /// <summary>
    /// Configures attributes for the class.
    /// </summary>
    /// <param name="attributeConfigurator">A configurator action for attributes.</param>
    /// <returns>An instance of IClassAttributesConfigured for chaining.</returns>
    IClassAttributesConfigured Attributes(Action<AttributeBuilder> attributeConfigurator);
}

/// <summary>
/// Represents a stage in the class building process where class attributes are configured.
/// </summary>
public interface IClassAttributesConfigured : IClassBaseConfigured
{
    /// <summary>
    /// Specifies a base class for the class being built.
    /// </summary>
    /// <typeparam name="TBase">Type of the base class.</typeparam>
    /// <returns>An instance of IClassBaseConfigured for chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception> 
    IClassBaseConfigured Extends<TBase>() where TBase : class;
}

/// <summary>
/// Represents a stage in the class building process where the base class and interfaces are configured.
/// </summary>
public interface IClassBaseConfigured : IClassInterfacesConfigured, IClassPropertiesConfigured
{
    /// <summary>
    /// Allows the class to implement specified interfaces.
    /// </summary>
    /// <typeparam name="TInterface">Type of the interface to implement.</typeparam>
    /// <returns>An instance of IClassInterfacesConfigured for chaining.</returns>
    IClassInterfacesConfigured Implements<TInterface>() where TInterface : class;
}

/// <summary>
/// Represents a stage in the class building process focused on interface implementation and constructor addition.
/// </summary>
public interface IClassInterfacesConfigured : IClassBuilderAddMethods
{
    /// <summary>
    /// Adds a default constructor to the class.
    /// </summary>
    /// <returns>An instance of IClassBuilderAddMethods for chaining.</returns>
    IClassBuilderAddMethods Constructor();

    /// <summary>
    /// Adds a constructor to the class, configured with the provided action.
    /// </summary>
    /// <param name="constructorConfigurator">The action to configure the constructor.</param>
    /// <returns>An instance of IClassBuilderAddMethods for chaining.</returns>
    IClassBuilderAddMethods Constructor(Action<ConstructorBuilder> constructorConfigurator);

    /// <summary>exted
    /// Configures properties for the class.
    /// </summary>
    /// <param name="propertyConfigurator">The action to configure class properties.</param>
    /// <returns>An instance of IClassPropertiesConfigured for chaining.</returns>
    IClassPropertiesConfigured Properties(Action<PropertyBuilder> propertyConfigurator);
}

/// <summary>
/// Represents a stage in the class building process focused on property configuration.
/// </summary>
public interface IClassPropertiesConfigured : IClassBuilderAddMethods
{
    /// <summary>
    /// Adds a default constructor to the class.
    /// </summary>
    /// <returns>An instance of IClassBuilderAddMethods for chaining.</returns>
    IClassBuilderAddMethods Constructor();

    /// <summary>
    /// Adds a constructor to the class, configured with the provided action.
    /// </summary>
    /// <param name="constructorConfigurator">The action to configure the constructor.</param>
    /// <returns>An instance of IClassBuilderAddMethods for chaining.</returns>
    IClassBuilderAddMethods Constructor(Action<ConstructorBuilder> constructorConfigurator);
}

/// <summary>
/// Represents a stage in the class building process for adding methods.
/// </summary>
public interface IClassBuilderAddMethods
{
    /// <summary>
    /// Adds a method to the class with the specified name, configured with the provided action.
    /// </summary>
    /// <param name="methodName">The name of the method to add.</param>
    /// <param name="methodConfigurator">The action to configure the method.</param>
    /// <returns>An instance of IClassBuilderAddMethods for chaining.</returns>
    IClassBuilderAddMethods AddMethod(string methodName, Action<MethodBuilder> methodConfigurator);

    /// <summary>
    /// Adds a method to the class with the specified name and return type, configured with the provided action.
    /// </summary>
    /// <typeparam name="T">The return type of the method.</typeparam>
    /// <param name="methodName">The name of the method to add.</param>
    /// <param name="methodConfigurator">The action to configure the method.</param>
    /// <returns>An instance of IClassBuilderAddMethods for chaining.</returns>
    IClassBuilderAddMethods AddMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator);
}
