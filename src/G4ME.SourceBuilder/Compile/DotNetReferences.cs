using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

public class DotNetReferences : IEnumerable<PortableExecutableReference>
{
    private readonly HashSet<PortableExecutableReference> _references = [];

    public DotNetReferences()
    {
        // Basic check for .NET Core or .NET 5/6/7
        if (IsNetCore)
        {
            // For .NET Core and .NET 5+, add references like this:
            Add("System.Runtime");
            Add("System.Console");
            Add<object>();

            // Additional references can be added as needed
        }
        else
        {
            // For .NET Framework, add specific references:
            Add("mscorlib");
            Add("System");
            Add("System.Core"); // System.dll
        }
    }

    public IEnumerator<PortableExecutableReference> GetEnumerator() => _references.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static bool IsNetCore => Environment.Version.Major >= 5 || RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);

    private void Add<T>() => Add(typeof(T));

    private void Add(Type type)
    {
        _references.Add(MetadataReference.CreateFromFile(type.Assembly.Location));
    }

    private void Add(string name) => _references.Add(Reference(name));

    private static PortableExecutableReference Reference(string name) => MetadataReference.CreateFromFile(Assembly.Load(name).Location);
    
}
