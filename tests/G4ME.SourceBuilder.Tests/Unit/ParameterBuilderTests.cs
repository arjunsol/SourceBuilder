namespace G4ME.SourceBuilder.Tests.Unit;

public class ParameterBuilderTests
{
    [Fact]
    public void AddParameter_ShouldAddSingleParameter()
    {
        var builder = new ParameterBuilder();
        builder.AddParameter<int>("param1");

        var parameterList = builder.Build();

        Assert.Single(parameterList.Parameters);
        var parameter = parameterList.Parameters[0];

        Assert.NotNull(parameter.Type);
        Assert.Equal("param1", parameter.Identifier.ValueText);        
        Assert.Equal("int", parameter.Type.ToString());
    }

    [Fact]
    public void AddParameter_ShouldAddMultipleParameters()
    {
        var builder = new ParameterBuilder();
        builder.AddParameter<int>("param1");
        builder.AddParameter<string>("param2");

        var parameterList = builder.Build();

        Assert.Equal(2, parameterList.Parameters.Count);

        var parameter0 = parameterList.Parameters[0];
        var parameter1 = parameterList.Parameters[1];

        Assert.NotNull(parameter0.Type);
        Assert.Equal("param1", parameter0.Identifier.ValueText);
        Assert.Equal("int", parameter0.Type.ToString());

        Assert.NotNull(parameter1.Type);
        Assert.Equal("param2", parameter1.Identifier.ValueText);
        Assert.Equal("string", parameter1.Type.ToString());
    }

    [Fact]
    public void AddParameter_ShouldHandleDifferentParameterTypes()
    {
        var builder = new ParameterBuilder();
        builder.AddParameter<double>("paramDouble");

        var parameterList = builder.Build();

        Assert.Single(parameterList.Parameters);

        var parameter0 = parameterList.Parameters[0];

        Assert.NotNull(parameter0.Type);
        Assert.Equal("paramDouble", parameter0.Identifier.ValueText);
        Assert.Equal("double", parameter0.Type.ToString());
    }

    [Fact]
    public void Build_ShouldReturnCorrectParameterListSyntax()
    {
        var builder = new ParameterBuilder();
        builder.AddParameter<int>("param1");
        builder.AddParameter<string>("param2");

        var parameterList = builder.Build();

        Assert.Equal(2, parameterList.Parameters.Count);        
    }
}

