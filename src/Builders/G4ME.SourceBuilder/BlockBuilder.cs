namespace G4ME.SourceBuilder;

public class BlockBuilder
{
    private readonly List<StatementSyntax> _statements = [];

    public BlockBuilder AddStatement(string statement)
    {
        var parsedStatement = SyntaxFactory.ParseStatement(statement);
        _statements.Add(parsedStatement);
        return this;
    }

    public BlockSyntax Build()
    {
        return SyntaxFactory.Block(_statements);
    }
}
