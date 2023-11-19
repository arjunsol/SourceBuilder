using System.Reflection;

namespace G4ME.SourceBuilder.Tests.Compile;

public class Compiler
{
    public object GetInstance(CompilationUnitBuilder compilationUnitBuilder, string className, params object[] constructorArgs)
    {
        CompilationUnitSyntax compilationUnit = compilationUnitBuilder.Build();
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(compilationUnit.NormalizeWhitespace().ToFullString());

        var compilation = CSharpCompilation.Create("DynamicAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(compilationUnitBuilder.GetRequiredReferences())
            .AddSyntaxTrees(syntaxTree);

        Assembly assembly = CompileAndLoadAssembly(compilation);
        
        return CreateInstanceOfCompiledClass(assembly, className, constructorArgs);
    }

    private Assembly CompileAndLoadAssembly(Compilation compilation)
    {
        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        var errors = compilation.GetDiagnostics()
                            .Where(diagnostic => diagnostic.IsWarningAsError ||
                                                 diagnostic.Severity == DiagnosticSeverity.Error)
                            .Select(diagnostic => $"{diagnostic.Id}: {diagnostic.GetMessage()}")
                            .ToList();

        if (errors.Any())
        {
            throw new InvalidOperationException($"Compilation failed: {string.Join("\n", errors)}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
    }

    private object CreateInstanceOfCompiledClass(Assembly assembly, string className, params object[] constructorArgs)
    {
        Type? type = assembly.GetType(className);

        if (type is null)
        {
            throw new InvalidOperationException($"Type {className} not found in assembly.");
        }

        return Activator.CreateInstance(type, constructorArgs) ?? throw new InvalidOperationException();
    }
}
