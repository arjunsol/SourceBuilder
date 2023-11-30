using System.Text.Json.Serialization;
using G4ME.SourceBuilder.Tests.Objects;
using Microsoft.CodeAnalysis;

namespace G4ME.SourceBuilder.Tests.Verified;

[UsesVerify]
public class VerifyClassBuilder
{
    [Fact]
    public async Task Create_SimpleClass_Verified()
    {
        var classBuilder = new ClassBuilder("SimpleClass");

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithAttribute_Verified()
    {
        ClassBuilder classBuilder = new("TestClass");

        classBuilder.Attributes(a =>
            a.Add<SerializableAttribute>());

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithDefaultConstrcutor_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass");
        
        classBuilder.Constructor();

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithConstrcutorBody_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass");
        
        classBuilder.Constructor(c => c
                                   .Body(b => b
                                   .AddLine(@"var thing = ""MyString"";")));

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithConstuctorBody_Verified()
    {
        ClassBuilder classBuilder = new("TestClass");
        
        classBuilder.Constructor(c => c
            .Body(@"var thing = ""MyString"";"));

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithConstrcutorMultilineBody_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass");
        classBuilder.Constructor(c => c
            .Body(b => b
            .AddLines(@"var thing = ""MyString"";",
                        @"var thung = thing;")));

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithProperty_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass");

        classBuilder.Properties(p => p.Add<string>("Name").Get().Set());

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithVoidMethod_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass");
        
        classBuilder.AddMethod("TestMethod", m => m
            .Body(b => b
                .AddLine(@"Console.WriteLine(""Hello World!"")")));

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithTypedMethod_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass");
        
        classBuilder.AddMethod("TestMethod", m => m
            .Body(b => b
                .AddLine(@"Console.WriteLine(GetMessage());")))
        .AddMethod<string>("GetMessage", m => m
            .Body(b => b
                .AddLine(@"return ""Hello World!"";")));

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ComplexClass_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass");

        classBuilder.Attributes(a => a
            .Add<JsonSerializableAttribute>("typeof(TestMethod)")
            .Add<AnotherAttribute>())
        .Extends<SomeClass>().Implements<ISomeInterface>()
        .Properties(p => p
            .AddPrivate<string>("Name").Get().Set())
        .Constructor(c => c
            .Parameter<int>("id")
            .Parameter<string>("name").MapBase()
            .Body("Name = name;"))
        .AddMethod("TestMethod", m => m //TODO: Methods (like properties and attributes)
            .Body(@"Console.WriteLine(Message);"))
        .AddMethod("SetMessage", m => m
            .Parameter<string>("message")
            .Body(b => b
            .AddLine(@"Message = message;")));

        await BuildAndVerify(classBuilder);
    }

    private static async Task BuildAndVerify(ITypeBuilder<TypeDeclarationSyntax> classBuilder)
    {
        var generatedClass = classBuilder.Build();

        await Verify(generatedClass.NormalizeWhitespace().ToFullString());
    }
}
