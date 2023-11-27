using System.Collections;
using System.Reflection;

namespace G4ME.SourceBuilder.Compile;

public class ReferenceCollection : IEnumerable<PortableExecutableReference>
{
    private readonly HashSet<PortableExecutableReference> _assemblyLocations = [ ];

    public void AddDotNetReferences()
    {
        Add(Assembly.Load("System.Runtime").Location);
        Add(Assembly.Load("System.Console").Location);
        Add<object>();
    }

    public void Add<T>() => Add(typeof(T));

    public void Add(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (IsPrimitive(type)) return;

        string assemblyLocation = type.Assembly.Location;

        Add(assemblyLocation);

        if (type.IsGenericType)
        {
            foreach (var argument in type.GetGenericArguments())
            {
                Add(argument);
            }
        }
    }

    public void Add(string path)
    {
        ValidateAssemblyPath(path);

        _assemblyLocations.Add(MetadataReference.CreateFromFile(path));
    }

    public IEnumerator<PortableExecutableReference> GetEnumerator() => _assemblyLocations.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void UnionWith(ReferenceCollection other)
    {
        ArgumentNullException.ThrowIfNull(other);

        _assemblyLocations.UnionWith(other);
    }

    private void ValidateAssemblyPath(string path)
    {
        //TODO: Guard clauses
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Assembly path cannot be null or whitespace.", nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The assembly file at '{path}' was not found.");
        }
    }

    private static bool IsPrimitive(Type type) =>
                            type.IsPrimitive ||
                            type == typeof(string) ||
                            type == typeof(decimal);
}
