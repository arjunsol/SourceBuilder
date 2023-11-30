using G4ME.SourceBuilder.Compile;
using Microsoft.CodeAnalysis.CSharp.Syntax; // Assuming TypeDeclarationSyntax is part of this namespace

namespace G4ME.SourceBuilder.Syntax;

/// <summary>
/// Defines a generic interface for building types (classes, interfaces, records, etc.).
/// </summary>
/// <typeparam name="T">The type of the syntax representation for the type being built, derived from TypeDeclarationSyntax.</typeparam>
public interface ITypeBuilder<out T> where T : TypeDeclarationSyntax
{
    /// <summary>
    /// Gets the namespace of the type being built.
    /// </summary>
    string Namespace { get; }

    /// <summary>
    /// Gets the name of the type being built.
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// Builds and returns the type declaration syntax for the type.
    /// </summary>
    /// <returns>The constructed type declaration syntax.</returns>
    T Build();

    /// <summary>
    /// Adds a requirement for the type being built.
    /// </summary>
    /// <typeparam name="R">The requirement type.</typeparam>
    void AddRequirement<R>();

    /// <summary>
    /// Retrieves the requirements necessary for the type being built.
    /// </summary>
    /// <returns>The requirements for the type.</returns>
    Requirements GetRequirements();

    /// <example>
    /// Example of using ITypeBuilder to build a class type:
    /// <code>
    /// ITypeBuilder<ClassDeclarationSyntax> classBuilder = new ClassBuilder("MyClass", "MyNamespace");
    /// classBuilder.AddRequirement&lt;SomeDependency&gt;();
    /// ClassDeclarationSyntax classDeclaration = classBuilder.Build();
    /// Console.WriteLine(classDeclaration.ToString());
    /// </code>
    /// </example>
}
