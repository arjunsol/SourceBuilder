using System.Reflection;
using G4ME.SourceBuilder.Tests.Objects;

namespace G4ME.SourceBuilder.Tests.Architecture;

// TODO: Potentially write a library to help with reflection testing
public class ClassBuilderFluentInterfaceTests
{
    // Test for ClassBuilder
    [Fact]
    public void IClassBuilder_Attributes_Method_ShouldReturn_IClassAttributesConfigured()
    {
        AssertReturnType<IClassBuilder, IClassAttributesConfigured>(
            nameof(IClassBuilder.Attributes),
            new[] { typeof(Action<AttributeBuilder>) });
    }

    // Tests for IClassAttributesConfigured
    [Fact]
    public void IClassAttributesConfigured_Extends_Method_ShouldReturn_IClassBaseConfigured()
    {
        AssertReturnType<IClassAttributesConfigured, IClassBaseConfigured>(
            nameof(IClassAttributesConfigured.Extends));
    }

    [Fact]
    public void IClassAttributesConfigured_ShouldNotHave_AttributesMethod()
    {
        AssertMethodNotExists<IClassAttributesConfigured>(nameof(IClassBuilder.Attributes));
    }

    // Tests for IClassBaseConfigured
    [Fact]
    public void IClassBaseConfigured_Implements_Method_ShouldReturn_IClassInterfacesConfigured()
    {
        AssertReturnType<IClassBaseConfigured, IClassInterfacesConfigured>(
            nameof(IClassBaseConfigured.Implements));
    }

    [Fact]
    public void IClassBaseConfigured_ShouldNotHave_ExtendsMethod()
    {
        AssertMethodNotExists<IClassBaseConfigured>(nameof(IClassAttributesConfigured.Extends));
    }

    // Tests for IClassInterfacesConfigured
    [Fact]
    public void IClassInterfacesConfigured_Constructor_Method_ShouldReturn_IClassBuilderAddMethods()
    {
        AssertReturnType<IClassInterfacesConfigured, IClassBuilderAddMethods>(
            nameof(IClassInterfacesConfigured.Constructor),
            Type.EmptyTypes);
    }

    // Overloaded Constructor with parameters
    [Fact]
    public void IClassInterfacesConfigured_ConstructorWithParameters_Method_ShouldReturn_IClassBuilderAddMethods()
    {
        AssertReturnType<IClassInterfacesConfigured, IClassBuilderAddMethods>(
            nameof(IClassInterfacesConfigured.Constructor),
            new[] { typeof(Action<ConstructorBuilder>) });
    }

    [Fact]
    public void IClassInterfacesConfigured_ShouldNotHave_ImplementsMethod()
    {
        AssertMethodNotExists<IClassInterfacesConfigured>(nameof(IClassBaseConfigured.Implements));
    }

    // Tests for IClassPropertiesConfigured
    [Fact]
    public void IClassPropertiesConfigured_Constructor_Method_ShouldReturn_IClassBuilderAddMethods()
    {
        AssertReturnType<IClassPropertiesConfigured, IClassBuilderAddMethods>(
            nameof(IClassPropertiesConfigured.Constructor),
            Type.EmptyTypes);
    }

    [Fact]
    public void IClassPropertiesConfigured_ConstructorWithParameters_Method_ShouldReturn_IClassBuilderAddMethods()
    {
        AssertReturnType<IClassPropertiesConfigured, IClassBuilderAddMethods>(
            nameof(IClassPropertiesConfigured.Constructor),
            new[] { typeof(Action<ConstructorBuilder>) });
    }

    [Fact]
    public void IClassPropertiesConfigured_ShouldNotHave_PropertiesMethod()
    {
        AssertMethodNotExists<IClassPropertiesConfigured>(nameof(IClassBaseConfigured.Properties));
    }

    // Tests for IClassBuilderAddMethods
    [Fact]
    public void IClassBuilderAddMethods_AddMethod_ShouldReturn_IClassBuilderAddMethods()
    {
        AssertReturnType<IClassBuilderAddMethods, IClassBuilderAddMethods>(
            nameof(IClassBuilderAddMethods.AddMethod),
            [typeof(string), typeof(Action<MethodBuilder>)]);
    }
    
    private void AssertReturnType<TInterface, TReturnType>(string methodName)
    {
        MethodInfo methodInfo = typeof(TInterface).GetMethod(methodName);
        Assert.NotNull(methodInfo);
        Assert.Equal(typeof(TReturnType), methodInfo.ReturnType);
    }


    // Helper Methods
    private void AssertReturnType<TInterface, TReturnType>(string methodName, Type[] parameterTypes, bool isGenericMethod = false)
    {
        MethodInfo[] methods = typeof(TInterface).GetMethods()
            .Where(m => m.Name == methodName && m.IsGenericMethod == isGenericMethod)
            .ToArray();

        MethodInfo methodInfo = methods.Length == 1 ? methods[0] : methods.SingleOrDefault(m => m.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes));

        Assert.NotNull(methodInfo);
        Assert.Equal(typeof(TReturnType), methodInfo.ReturnType);
    }


    private void AssertMethodNotExists<TInterface>(string methodName)
    {
        MethodInfo methodInfo = typeof(TInterface).GetMethod(methodName);
        Assert.Null(methodInfo);
    }
}


