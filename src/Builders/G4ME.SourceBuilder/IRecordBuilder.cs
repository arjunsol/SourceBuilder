
namespace G4ME.SourceBuilder;

public interface IRecordBuilder : ITypeBuilder
{
    RecordBuilder ImplementsInterface<TInterface>() where TInterface : class;
    RecordBuilder InheritsFrom<TBase>() where TBase : class;
    RecordBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator);
    RecordBuilder WithMethod(string methodName, Action<MethodBuilder> methodConfigurator);
    RecordBuilder WithMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator);
    RecordBuilder WithProperty<T>(string propertyName, Action<PropertyBuilder> propertyConfigurator);
}
