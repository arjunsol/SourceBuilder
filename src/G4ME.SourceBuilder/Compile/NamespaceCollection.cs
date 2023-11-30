using System.Collections;

namespace G4ME.SourceBuilder.Compile;

public class NamespaceCollection(string currentNamespace) : IEnumerable<string>
{
    private readonly HashSet<string> _namespaces = [ ];
    
    public void Add<T>() => Add(typeof(T));

    public void Add(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (ValidNamespace(type) && !string.IsNullOrWhiteSpace(type.Namespace))
        {
            _namespaces.Add(type.Namespace);
        }

        if (type.IsGenericType)
        {
            foreach (var argument in type.GetGenericArguments())
            {
                Add(argument);
            }
        }
    }

    public void Add(string name) { }

    public bool ContainsNamespace(string namespaceName) => _namespaces.Contains(namespaceName);

    public void UnionWith(NamespaceCollection other)
    {
        ArgumentNullException.ThrowIfNull(other);

        _namespaces.UnionWith(other);        
    }

    private bool ValidNamespace(Type type) =>
                    !IsPrimitive(type) ||
                    type.Namespace != null ||
                    type.Namespace != currentNamespace ||
                    !_namespaces.Contains(type.Namespace);

    private static bool IsPrimitive(Type type) =>
                            type.IsPrimitive ||
                            type == typeof(string) ||
                            type == typeof(decimal);

    public IEnumerator<string> GetEnumerator() => _namespaces.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();    
}
