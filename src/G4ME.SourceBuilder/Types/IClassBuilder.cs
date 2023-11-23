namespace G4ME.SourceBuilder.Types;

public interface IClassBuilder : ITypeBuilder
{
    ClassBuilder Implements<TInterface>() where TInterface : class;
    ClassBuilder Extends<TBase>() where TBase : class;
    ClassBuilder Attributes(Action<AttributeBuilder> attributeConfigurator);
    ClassBuilder Constructor(Action<ConstructorBuilder> constructorConfigurator);
    //ClassBuilder WithIndexer(IndexerDeclarationSyntax indexerDeclaration);
    ClassBuilder AddMethod(string methodName, Action<MethodBuilder> methodConfigurator);
    ClassBuilder AddMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator);
    ClassBuilder Properties(Action<PropertyBuilder> propertyConfigurator);
}
