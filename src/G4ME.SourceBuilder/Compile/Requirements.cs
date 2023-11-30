
namespace G4ME.SourceBuilder.Compile;

public class Requirements(string currentNamespace)
{
    private NamespaceCollection _namespaces = new(currentNamespace);
    private ReferenceCollection _references = [ ];

    public NamespaceCollection Namespaces => _namespaces;
    public ReferenceCollection References => _references;

    public void Add<T>()
    {
        _namespaces.Add<T>();
        _references.Add<T>();
    }

    public void Add(Type type)
    {
        _namespaces.Add(type);
        _references.Add(type);
    }

    public void Add(string name) 
    {
        _namespaces.Add(name);
        _references.Add(name);
    }

    public void UnionWith(Requirements other)
    {
        _namespaces.UnionWith(other.Namespaces);
        _references.UnionWith(other.References);
    }
}
