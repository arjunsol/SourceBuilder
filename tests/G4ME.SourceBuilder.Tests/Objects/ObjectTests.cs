namespace G4ME.SourceBuilder.Tests.Objects;

public class ObjectTests
{
    [Fact]
    public void BaseRecord_Construct()
    {
        var baseRecord = new BaseRecord(1);

        Assert.Equal(1, baseRecord.Id);
    }

    [Fact]
    public void SomeClass_Construct()
    {
        var someClass = new SomeClass(1, "param2");

        Assert.Equal(1, someClass.Property1);
        Assert.Equal("param2", someClass.Property2);
    }

    [Fact]
    public void SomeAttribute_Construct()
    {
        var attr = new SomeAttribute("");

        Assert.Equal("", attr.Args[0]);
    }

    [Fact]
    public void AnotherAttribute_Construct()
    {
        var attr = new AnotherAttribute("");

        Assert.Equal("", attr.Args[0]);
    }
}
