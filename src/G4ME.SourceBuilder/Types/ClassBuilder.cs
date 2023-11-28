﻿using G4ME.SourceBuilder.Compile;

namespace G4ME.SourceBuilder.Syntax;

public class ClassBuilder(string name, string classNamespace) : IClassBuilder
{
    private readonly Requirements _requirements = new(classNamespace);
    private ClassDeclarationSyntax _classDeclaration = SyntaxFactory.ClassDeclaration(name)
                                                                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
    
    public string TypeName { get; private set; } = name;
    public string Namespace => classNamespace;

    public ClassBuilder(string name) : this(name, string.Empty) { }

    public ClassBuilder Extends<TBase>() where TBase : class
    {
        AddRequirement<TBase>();

        var baseTypeName = Syntax.TypeName.ValueOf<TBase>();
        _classDeclaration = _classDeclaration.AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeName))
        );

        return this;
    }

    public ClassBuilder Implements<TInterface>() where TInterface : class
    {
        if (!typeof(TInterface).IsInterface)
        {
            throw new ArgumentException("Generic type must be an interface.", nameof(TInterface));
        }

        AddRequirement<TInterface>();

        Type interfaceType = typeof(TInterface);

        string interfaceName = interfaceType.IsGenericType
                            ? GetGenericTypeName(interfaceType)
                            : interfaceType.Name;

        _classDeclaration = _classDeclaration.AddBaseListTypes(
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(interfaceName)));

        return this;
    }

    public ClassBuilder Attributes(Action<AttributeBuilder> attributeConfigurator)
    {
        AttributeBuilder attributeBuilder = new(this);

        attributeConfigurator(attributeBuilder);
        _classDeclaration = _classDeclaration.WithAttributeLists(attributeBuilder.Build());

        return this;
    }

    public ClassBuilder Properties(Action<PropertyBuilder> propertyConfigurator)
    {
        PropertyBuilder propertyBuilder = new();
        
        propertyConfigurator(propertyBuilder);
        _classDeclaration = _classDeclaration.AddMembers(propertyBuilder.Build());

        return this;
    }

    public ClassBuilder Constructor()
    {
        ConstructorBuilder constructorBuilder = new(this);
        _classDeclaration = _classDeclaration.AddMembers(constructorBuilder.Build());

        return this;
    }

    public ClassBuilder Constructor(Action<ConstructorBuilder> constructorConfigurator)
    {
        ConstructorBuilder constructorBuilder = new(this);
        constructorConfigurator(constructorBuilder);
        _classDeclaration = _classDeclaration.AddMembers(constructorBuilder.Build());

        return this;
    }

    public ClassBuilder AddMethod(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        MethodBuilder methodBuilder = new(this, methodName);

        methodConfigurator(methodBuilder);
        _classDeclaration = _classDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public ClassBuilder AddMethod<T>(string methodName, Action<MethodBuilder> methodConfigurator)
    {
        AddRequirement<T>();
                
        string returnType = Syntax.TypeName.ValueOf<T>();

        MethodBuilder methodBuilder = new(this, returnType, methodName);
        methodConfigurator(methodBuilder);
        _classDeclaration = _classDeclaration.AddMembers(methodBuilder.Build());

        return this;
    }

    public void AddRequirement<T>() => _requirements.Add<T>();

    //public ClassBuilder WithIndexer(IndexerDeclarationSyntax indexerDeclaration)
    //{
    //    _classDeclaration = _classDeclaration.AddMembers(indexerDeclaration);
    //    return this;
    //}

    
    public TypeDeclarationSyntax Build() => _classDeclaration;

    public Requirements GetRequirements() => _requirements;

    private string GetGenericTypeName(Type type)
    {
        Type[] genericArguments = type.GetGenericArguments();
        string typeNameWithoutArity = type.Name.Split('`')[0];
        string formattedGenericArguments = string.Join(", ", genericArguments.Select(arg => arg.Name));

        return $"{typeNameWithoutArity}<{formattedGenericArguments}>";
    }
}
