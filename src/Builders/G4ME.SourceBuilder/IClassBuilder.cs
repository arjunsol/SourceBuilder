namespace G4ME.SourceBuilder;

public interface IClassBuilder : ITypeBuilder
{
    ClassBuilder ImplementsInterface<TInterface>() where TInterface : class;
    ClassBuilder InheritsFrom<TBase>() where TBase : class;
    ClassBuilder WithAttributes(Action<AttributeBuilder> attributeConfigurator);
    ClassBuilder WithConstructor(Action<ConstructorBuilder> constructorConfigurator);
    //ClassBuilder WithIndexer(IndexerDeclarationSyntax indexerDeclaration);
    ClassBuilder WithMethod(string methodName, Action<MethodBuilder> methodConfigurator);
    ClassBuilder WithMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator);
    public ClassBuilder WithProperty<T>(string propertyName, Action<PropertyBuilder> propertyConfigurator);
}
