namespace G4ME.SourceBuilder.Syntax;

public interface IInterfaceBuilder : ITypeBuilder
{
    InterfaceBuilder InheritsFrom<TBase>() where TBase : class;
    InterfaceBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator);
    InterfaceBuilder WithMethod(string methodName, Action<MethodBuilder> methodConfigurator);
    InterfaceBuilder Properties(Action<PropertyBuilder> propertyConfigurator);
}
