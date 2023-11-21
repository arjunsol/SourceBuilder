namespace G4ME.SourceBuilder.Syntax;

public class NamespaceCollection(string currentNamespace)
{
    private readonly HashSet<string> _namespaces = [];

    public void Add<T>()
    {
        Type type = typeof(T);

        Add(type);
    }

    private void Add(Type type)
    {
        if (InvalidNamespace(type)) return;

        ArgumentNullException.ThrowIfNull(type.Namespace);

        _namespaces.Add(type.Namespace);

        if (type.IsGenericType)
        {
            foreach (var argument in type.GetGenericArguments())
            {
                Add(argument);
            }
        }
    }

    public IEnumerable<string> GetAll() => _namespaces;

    private bool InvalidNamespace(Type type) => 
                    IsPrimitive(type) ||
                    type.Namespace == null ||
                    type.Namespace == currentNamespace ||
                    _namespaces.Contains(type.Namespace);

    private static bool IsPrimitive(Type type) => 
                            type.IsPrimitive ||
                            type == typeof(string) ||
                            type == typeof(decimal);
}
