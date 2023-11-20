namespace G4ME.SourceBuilder.Tests;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class SomeAttribute(params object[] args) : Attribute
{
    public object[] Args { get; } = args;
}

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class AnotherAttribute(params object[] args) : Attribute
{
    public object[] Args { get; } = args;
}

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class TestAttribute(params object[] args) : Attribute
{
    public object[] Args { get; } = args;
}
