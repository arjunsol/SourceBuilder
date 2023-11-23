namespace G4ME.SourceBuilder.Syntax;

public class BlockBuilder
{
    private readonly List<StatementSyntax> _statements = [];

    public BlockBuilder AddLines(params string[] statements)
    {
        foreach (var statement in statements) AddLine(statement);

        return this;
    }

    public BlockBuilder AddLine(string statement)
    {
        // Ensure the statement ends with a semicolon
        if (!statement.TrimEnd().EndsWith(';'))
        {
            statement += ";";
        }

        var parsedStatement = SyntaxFactory.ParseStatement(statement);
        _statements.Add(parsedStatement);
        
        return this;
    }

    public BlockSyntax Build() => SyntaxFactory.Block(_statements);
}
