using System.Reflection;
using G4ME.SourceBuilder.Tests.Objects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace G4ME.SourceBuilder.Tests.Compile;

public class ClassCompilationTests
{
    private Compiler _compiler = new();

    [Fact]
    public void Compilation_SimpleClass_NoErrors()
    {
        const string CLASSNAME = "MyClass";
        const string NAMESPACE = "MyNamespace";

        ClassBuilder classBuilder = new(CLASSNAME, NAMESPACE);

        CompilationUnitBuilder compilationUnitBuilder = new(classBuilder);

        CompilationUnitSyntax compilationUnit = compilationUnitBuilder.Build();
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(compilationUnit.NormalizeWhitespace().ToFullString());

        object instance = _compiler.GetInstance(compilationUnitBuilder, $"{NAMESPACE}.{CLASSNAME}");

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
        
        object instance = _compiler.GetInstance(compilationUnitBuilder, $"{NAMESPACE}.{CLASSNAME}", 1, "Nick");

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
        
        Assert.Throws<InvalidOperationException>(() => _compiler.GetInstance(compilationUnitBuilder, "MyNamespace.MyClass"));
    }
}
