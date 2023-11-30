namespace G4ME.SourceBuilder.Syntax;

/// <summary>
/// Defines the contract for a builder that constructs record types.
/// It inherits from IRecordAttributesConfigured and ITypeBuilder to facilitate additional configuration capabilities.
/// </summary>
public interface IRecordBuilder : IRecordAttributesConfigured, ITypeBuilder<RecordDeclarationSyntax>
{
    /// <summary>
    /// Configures attributes for the record.
    /// </summary>
    /// <param name="attributeConfigurator">An action that takes an AttributeBuilder to configure attributes for the record.</param>
    /// <returns>An instance of IRecordAttributesConfigured to allow method chaining.</returns>
    IRecordAttributesConfigured Attributes(Action<AttributeBuilder> attributeConfigurator);
}

/// <summary>
/// Provides functionality for configuring attributes of a record.
/// This interface allows for chaining further configurations related to the base type of the record.
/// </summary>
public interface IRecordAttributesConfigured : IRecordBaseConfigured
{
    /// <summary>
    /// Specifies the base class from which the record will extend.
    /// </summary>
    /// <typeparam name="TBase">The base class type.</typeparam>
    /// <returns>An instance of IRecordBaseConfigured to allow method chaining.</returns>
    IRecordBaseConfigured Extends<TBase>() where TBase : class;

    /// <summary>
    /// Adds a parameter to the record.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>An instance of IRecordAttributesConfigured to allow method chaining.</returns>
    IRecordAttributesConfigured Parameter<T>(string parameterName);
}

/// <summary>
/// Interface for configuring the base class and interfaces that the record implements.
/// </summary>
public interface IRecordBaseConfigured
{
    /// <summary>
    /// Specifies an interface that the record will implement.
    /// </summary>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <returns>An instance of IRecordBaseConfigured to allow method chaining.</returns>
    IRecordBaseConfigured Implements<TInterface>() where TInterface : class;
}
