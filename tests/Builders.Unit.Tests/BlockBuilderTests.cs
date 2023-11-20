namespace G4ME.SourceBuilder.Tests;

public class BlockBuilderTests
{
    [Fact]
    public void TestAddStatement_AddsStatementCorrectly()
    {
        var blockBuilder = new BlockBuilder();
        blockBuilder.AddStatement("int x = 1;");

        var block = blockBuilder.Build();
        var statement = block.Statements.First();

        Assert.Single(block.Statements);
        Assert.Equal("int x = 1;", statement.ToString());
    }

    [Fact]
    public void TestAddMultipleStatements_AddsAllStatements()
    {
        var blockBuilder = new BlockBuilder();
        blockBuilder.AddStatement("int x = 1;");
        blockBuilder.AddStatement("int y = 2;");

        var block = blockBuilder.Build();

        Assert.Equal(2, block.Statements.Count);
    }

    [Fact]
    public void TestBuild_BuildsBlockCorrectly()
    {
        var blockBuilder = new BlockBuilder();
        blockBuilder.AddStatement("int x = 1;");

        var block = blockBuilder.Build();

        Assert.NotNull(block);
        Assert.Single(block.Statements);
    }

    [Fact]
    public void TestBuildWithoutStatements_ReturnsEmptyBlock()
    {
        var blockBuilder = new BlockBuilder();
        var block = blockBuilder.Build();

        Assert.NotNull(block);
        Assert.Empty(block.Statements);
    }

    // Additional test methods go here...

    // Consider adding tests for handling other specific cases if applicable.
}
