using System.Reflection;
using G4ME.SourceBuilder.Tests.Objects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace G4ME.SourceBuilder.Tests.Compile
{
    public class ClassCompilationTests
    {
        [Fact]
        public void Compilation_SimpleClass_NoErrors()
        {
            const string CLASSNAME = "MyClass";
            const string NAMESPACE = "MyNamespace";

            ClassBuilder classBuilder = new(CLASSNAME, NAMESPACE);

            CompilationUnitBuilder compilationUnitBuilder = new(classBuilder);

            CompilationUnitSyntax compilationUnit = compilationUnitBuilder.Build();
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(compilationUnit.NormalizeWhitespace().ToFullString());

            object instance = GetInstance(syntaxTree, $"{NAMESPACE}.{CLASSNAME}", compilationUnitBuilder.GetRequiredReferences());

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
                                   .AddMethod("TestMethod", m => m
                                       .Body(@"Console.WriteLine(Message);"))
                                   .AddMethod("SetMessage", m => m
                                       .Parameter<string>("message")
                                       .Body(b => b
                                       .AddLine(@"Message = message;")));

            CompilationUnitBuilder compilationUnitBuilder = new(classBuilder);
            
            CompilationUnitSyntax compilationUnit = compilationUnitBuilder.Build();
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(compilationUnit.NormalizeWhitespace().ToFullString());
            
            object instance = GetInstance(syntaxTree, $"{NAMESPACE}.{CLASSNAME}", compilationUnitBuilder.GetRequiredReferences(), 1, "Nick");

            Assert.NotNull(instance);
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

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var compilationUnitBuilder = new CompilationUnitBuilder();
            
            Assert.Throws<InvalidOperationException>(() => GetInstance(syntaxTree, "MyNamespace.MyClass", compilationUnitBuilder.GetRequiredReferences()));
        }

        private object GetInstance(SyntaxTree syntaxTree, string className, IEnumerable<MetadataReference> references, params object[] constructorArgs)
        {
            var compilation = CSharpCompilation.Create("DynamicAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
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
            Type type = assembly.GetType(className);

            if (type is null)
            {
                throw new InvalidOperationException($"Type {className} not found in assembly.");
            }

            return Activator.CreateInstance(type, constructorArgs) ?? throw new InvalidOperationException();
        }
    }
}
