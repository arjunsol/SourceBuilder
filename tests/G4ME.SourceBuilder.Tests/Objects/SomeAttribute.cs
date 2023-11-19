namespace G4ME.SourceBuilder.Tests.Objects;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class SomeAttribute(params object[] args) : Attribute
{
    public object[] Args { get; } = args;
}
