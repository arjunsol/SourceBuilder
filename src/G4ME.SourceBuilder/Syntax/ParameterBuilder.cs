namespace G4ME.SourceBuilder.Syntax;

public class ParameterBuilder
{
    private readonly List<ParameterSyntax> _parameters = [ ];

    public ParameterBuilder AddParameter<T>(string parameterName)
    {
        var parameterType = TypeName.ValueOf<T>();

        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
            .WithType(SyntaxFactory.ParseTypeName(parameterType));
        _parameters.Add(parameter);

        return this;
    }

    public ParameterListSyntax Build() => SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(_parameters));

}
