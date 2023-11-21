namespace G4ME.SourceBuilder.Syntax;

public class BlockBuilder
{
    private readonly List<StatementSyntax> _statements = [];

    public BlockBuilder AddStatement(string statement)
    {
        var parsedStatement = SyntaxFactory.ParseStatement(statement);
        _statements.Add(parsedStatement);
        return this;
    }

    public BlockSyntax Build() => SyntaxFactory.Block(_statements);
}
