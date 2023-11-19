namespace G4ME.SourceBuilder.Types;

/// <summary>
/// Defines the contract for a builder that constructs interface types.
/// Inherits from IInterfaceAttributesAdded and ITypeBuilder for additional configuration capabilities.
/// </summary>
public interface IInterfaceBuilder : IInterfaceAttributesAdded, ITypeBuilder<InterfaceDeclarationSyntax>
{
    /// <summary>
    /// Configures attributes for the interface.
    /// </summary>
    /// <param name="attributeConfigurator">An action that takes an AttributeBuilder to configure attributes for the interface.</param>
    /// <returns>An instance of IInterfaceAttributesAdded to allow method chaining.</returns>
    /// <example>
    /// This example demonstrates how to configure attributes for an interface. <see cref="Attributes"/>
    /// <code>
    /// .Atrributes(ab => ab.Add<SerializableAttribute>())
    /// </code>
    /// </example>
    IInterfaceAttributesAdded Attributes(Action<AttributeBuilder> attributeConfigurator);
}

/// <summary>
/// Provides functionality for configuring attributes of an interface.
/// Allows chaining further configurations related to the base interfaces that the interface extends.
/// </summary>
public interface IInterfaceAttributesAdded : IInterfaceExtended
{
    /// <summary>
    /// Specifies base interfaces that the interface will extend.
    /// </summary>
    /// <typeparam name="TBase">The base interface type.</typeparam>
    /// <returns>An instance of IInterfaceExtended to allow method chaining.</returns>
    /// <example>
    /// This example demonstrates how to specify base interfaces for an interface. <see cref="Extends{TBase}"/>
    /// <code>
    /// .Extends<IBaseInterface>()
    /// </code>
    /// </example>
    IInterfaceExtended Extends<TBase>() where TBase : class;
}

/// <summary>
/// Interface for configuring properties of an interface.
/// Allows chaining further configurations related to methods that the interface includes.
/// </summary>
public interface IInterfaceExtended : IInterfaceAddMethods
{
    /// <summary>
    /// Adds properties to the interface.
    /// </summary>
    /// <param name="propertyConfigurator">An action that takes a PropertyBuilder to configure properties for the interface.</param>
    /// <returns>An instance of IInterfaceAddMethods to allow method chaining.</returns>
    /// <example>
    /// This example demonstrates how to add properties to an interface. <see cref="Properties"/>
    /// <code>
    /// .Properties(pb => pb.Add<int>("Age").Get().Set())
    /// .properties(pb => pb.Add<string>("Name").Get().PrivateSet())
    /// </code>
    /// </example>
    IInterfaceAddMethods Properties(Action<PropertyBuilder> propertyConfigurator);
}

/// <summary>
/// Interface for adding methods to an interface.
/// </summary>
public interface IInterfaceAddMethods
{
    /// <summary>
    /// Adds a method to the interface.
    /// </summary>
    /// <param name="methodName">The name of the method to be added.</param>
    /// <param name="methodConfigurator">An action that takes an InterfaceMethodBuilder to configure the method.</param>
    /// <returns>An instance of IInterfaceAddMethods to allow further method additions.</returns>
    /// <example>
    /// This example demonstrates how to add a method to an interface. <see cref="AddMethod"/>
    /// <code>
    /// .AddMethod("MyMethod", mb => mb.Parameter<int>("age")
    /// </code>
    /// </example>
    IInterfaceAddMethods AddMethod(string methodName, Action<InterfaceMethodBuilder> methodConfigurator);

    /// <summary>
    /// Adds a method with a return type to the interface.
    /// </summary>
    /// <typeparam name="T">The return type of the method.</typeparam>
    /// <param name="methodName">The name of the method to be added.</param>
    /// <param name="methodConfigurator">An action that takes an InterfaceMethodBuilder to configure the method.</param>
    /// <returns>An instance of IInterfaceAddMethods to allow further method additions.</returns>
    /// <example>
    /// This example demonstrates how to add a method with a return type to an interface. <see cref="AddMethod{T}"/>
    /// <code>
    /// .AddMethod<int>("MyMethod", mb => mb.Parameter<int>("age")
    /// </code>
    /// </example>
    IInterfaceAddMethods AddMethod<T>(string methodName, Action<InterfaceMethodBuilder> methodConfigurator);
}
