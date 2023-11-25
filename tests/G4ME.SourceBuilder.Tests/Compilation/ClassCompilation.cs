using System.Reflection;
using G4ME.SourceBuilder.Tests.Objects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace G4ME.SourceBuilder.Tests.Compile;

// TODO: Rollup the compilation functionality into the main library
public class ClassCompilationTests
{
    [Fact]
    public void Compilation_SimpleClass_NoErrors()
    {
        const string CLASSNAME = "MyClass";
        const string NAMESPACE = "MyNamespace";

        ClassBuilder classBuilder = new(CLASSNAME, NAMESPACE);
        
        var syntaxTree = GetSyntaxTree(classBuilder);
        object instance = GetInstance(syntaxTree, $"{NAMESPACE}.{CLASSNAME}");

        Assert.NotNull(instance);
    }

    [Fact]
    public void Compilation_ComplexClass_NoErrors()
    {
        const string CLASSNAME = "MyClass";
        const string NAMESPACE = "MyNamespace";

        var classBuilder = new ClassBuilder(CLASSNAME, NAMESPACE)
                                   .Attributes(a => a
                                       .Add<SerializableAttribute>()
                                       .Add<AnotherAttribute>())
                                   .Extends<SomeClass>().Implements<ISomeInterface>()
                                   .Properties(p => p
                                       .AddPrivate<string>("Message").Get().Set())
                                   .Constructor(c => c
                                       .Parameter<int>("age") 
                                       .Parameter<string>("name")
                                       .MapBase()
                                       .Body("Message = name;"))                                   
                                   .AddMethod("TestMethod", m => m //TODO: Methods (like properties and attributes)
                                       .Body(@"Console.WriteLine(Message);"))
                                   .AddMethod("SetMessage", m => m
                                       .Parameter<string>("message")
                                       .Body(b => b
                                       .AddLine(@"Message = message;")));

        var syntaxTree = GetSyntaxTree(classBuilder);
        object instance = GetInstance(syntaxTree, $"{NAMESPACE}.{CLASSNAME}", 1, "Nick");

        Assert.NotNull(instance);
    }

    private SyntaxTree GetSyntaxTree(ClassBuilder classBuilder)
    {
        CompilationUnitBuilder compilationUnitBuilder = new(classBuilder);

        CompilationUnitSyntax compilationUnit = compilationUnitBuilder.Build();
        SyntaxTree syntaxTree = GetSyntaxTree(compilationUnit.NormalizeWhitespace().ToFullString());
        
        return syntaxTree;
    }

    [Fact]
    public void CompilationShouldFail()
    {
        string sourceCode = @"
                namespace MyNamespace;

                public class MyClass
                {
                    public void MyMethod()
                    {
                        int x = 10
                        // Missing semicolon will cause a syntax error
                    }
                }
            ";

        SyntaxTree syntaxTree = GetSyntaxTree(sourceCode);

        Assert.Throws<InvalidOperationException>(() => GetInstance(syntaxTree, "MyNamespace.MyClass"));
    }

    private object GetInstance(SyntaxTree syntaxTree, string className, params object[] constructorArgs)
    {
        var compilation = CSharpCompilation.Create("DynamicAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences( // TODO: These should be provided by CompilatioUnitBuilder
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // System.Private.CoreLib
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location), // System.Console, etc.
                MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location), // Roslyn
                MetadataReference.CreateFromFile(typeof(SomeClass).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(SerializableAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
                //MetadataReference.CreateFromFile(Assembly.Load("System.Console").Location)
            // Add other necessary references here
            ).AddSyntaxTrees(syntaxTree);

        Assembly assembly = CompileAndLoadAssembly(compilation);

        ListTypesInAssembly(assembly);

        // Replace 'MyClassName' with the name of the class you want to instantiate
        var instance = CreateInstanceOfCompiledClass(assembly, className, constructorArgs);

        return instance;        
    }

    // TODO: Reintroduce once we work out why compilationunits allow invalid syntax
    //private static SyntaxTree GetSyntaxTree(CompilationUnitSyntax compilationUnit) =>
    //                            CSharpSyntaxTree.Create(compilationUnit);

    private SyntaxTree GetSyntaxTree(string sourceCode) =>
                           CSharpSyntaxTree.ParseText(sourceCode);   



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
        Type type = assembly.GetType(className);

        if (type == null)
        {
            throw new InvalidOperationException($"Type {className} not found in assembly.");
        }

        return Activator.CreateInstance(type, constructorArgs);
    }

    //private object CreateInstance(Assembly assembly, string className, object[] constructorArgs = null)
    //{
    //    Type type = assembly.GetType(className);
    //    if (type == null)
    //        throw new InvalidOperationException($"Type {className} not found in assembly.");

    //    if (constructorArgs == null || constructorArgs.Length == 0)
    //    {
    //        return Activator.CreateInstance(type);
    //    }
    //    else
    //    {
    //        // Find a constructor that matches the provided arguments
    //        Type[] argTypes = constructorArgs.Select(arg => arg.GetType()).ToArray();
    //        ConstructorInfo constructorInfo = type.GetConstructor(argTypes);

    //        if (constructorInfo == null)
    //            throw new InvalidOperationException($"Constructor with specified arguments not found in type {className}.");

    //        return constructorInfo.Invoke(constructorArgs);
    //    }
    //}

    private void ListTypesInAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            Console.WriteLine(type.FullName);
        }
    }
}
