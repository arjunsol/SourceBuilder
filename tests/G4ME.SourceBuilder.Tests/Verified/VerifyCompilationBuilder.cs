using G4ME.SourceBuilder.Tests.Objects;
using Microsoft.CodeAnalysis;

namespace G4ME.SourceBuilder.Tests.Verified;

[UsesVerify]
public class VerifyCompilationBuilder
{
    [Fact]
    public async Task Create_SimpleClass_Verified()
    {
        const string CLASSNAME = "MyClass";
        const string NAMESPACE = "MyNamespace";

        ClassBuilder classBuilder = new(CLASSNAME, NAMESPACE);

        CompilationUnitBuilder compilationUnitBuilder = new(classBuilder);

        await BuildAndVerify(compilationUnitBuilder);
    }

    [Fact]
    public async Task Compilation_ComplexClass_NoErrors()
    {
        const string CLASSNAME = "MyClass";
        const string NAMESPACE = "MyNamespace";

        var classBuilder = new ClassBuilder(CLASSNAME, NAMESPACE);
        
        classBuilder.Attributes(a => a
                        .Add<SerializableAttribute>()
                        .Add<AnotherAttribute>())
                    .Extends<SomeClass>().Implements<ISomeInterface>()
                    .Properties(p => p
                        .AddPrivate<string>("Name").Get().Set())
                    .Constructor(c => c
                        .Parameter<int>("age")
                        .Parameter<string>("name")
                        .MapBase()
                        .Body("Name = name;"))
                    .AddMethod("TestMethod", m => m //TODO: Methods (like properties and attributes)
                        .Body(@"Console.WriteLine(Name);")) // TODO: Allow method overrides
                    .AddMethod("SetName", m => m
                        .Parameter<string>("name")
                        .Body(b => b
                        .AddLine(@"Name = name;")));
        


        CompilationUnitBuilder compilationUnitBuilder = new(classBuilder);

        await BuildAndVerify(compilationUnitBuilder);
    }

    private static async Task BuildAndVerify(CompilationUnitBuilder compilation)
    {
        var generatedTypes = compilation.Build();

        await Verify(generatedTypes.NormalizeWhitespace().ToFullString());
    }
}
