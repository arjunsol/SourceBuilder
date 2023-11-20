
namespace G4ME.SourceBuilder;

public interface IInterfaceBuilder : ITypeBuilder
{
    InterfaceBuilder InheritsFrom<TBase>() where TBase : class;
    InterfaceBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator);
    InterfaceBuilder WithMethod(string methodName, Action<MethodBuilder> methodConfigurator);
    InterfaceBuilder WithProperty<T>(string propertyName, Action<PropertyBuilder> propertyConfigurator);
}
