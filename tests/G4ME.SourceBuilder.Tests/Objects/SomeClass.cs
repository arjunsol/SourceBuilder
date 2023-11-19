namespace G4ME.SourceBuilder.Tests.Objects;

[Some("thing")]
[Another("thing")]
public class SomeClass(int property1, string property2)
{
    public int Property1 { get; set; } = property1;
    public string Property2 { get; set; } = property2;
}
