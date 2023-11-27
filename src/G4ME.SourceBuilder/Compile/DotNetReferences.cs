using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

public class DotNetReferences : IEnumerable<PortableExecutableReference>
{
    private readonly HashSet<PortableExecutableReference> references = [ ];

    public DotNetReferences()
    {
        // Basic check for .NET Core or .NET 5/6/7
        if (IsNetCore)
        {
            // For .NET Core and .NET 5+, add references like this:
            references.Add(MetadataReference.CreateFromFile(Assembly.Load("System").Location));
            // Additional references can be added as needed
        }
        else
        {
            // For .NET Framework, add specific references:
            references.Add(MetadataReference.CreateFromFile(Assembly.Load("mscorlib").Location));
            references.Add(MetadataReference.CreateFromFile(Assembly.Load("System").Location));
            references.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Core").Location)); // System.dll
        }
    }

    public IEnumerator<PortableExecutableReference> GetEnumerator() => references.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static bool IsNetCore => Environment.Version.Major >= 5 || RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);
    
}
