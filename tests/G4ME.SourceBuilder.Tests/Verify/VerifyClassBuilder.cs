using System.Text.Json.Serialization;
using G4ME.SourceBuilder.Tests.Objects;
using Microsoft.CodeAnalysis;

namespace G4ME.SourceBuilder.Tests;

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
        var classBuilder = new ClassBuilder("TestClass")
                               .WithAttributes(a => 
                                    a.AddAttribute<SerializableAttribute>());

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithDefaultConstrcutor_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass")
                               .WithConstructor(c => ConstructorBuilder.Empty());

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithConstrcutorBody_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass")
                               .WithConstructor(c => c
                                    .WithBody(b => b
                                        .AddStatement(@"var thing = ""MyString"";")));

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithConstrcutorMultilineBody_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass")
                               .WithConstructor(c => c
                                    .WithBody(b => b
                                        .AddStatements(@"var thing = ""MyString"";",    
                                                       @"var thung = thing;")));

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithProperty_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass")
                               .WithProperty<string>("Name", p => p
                                    .WithGetter()
                                    .WithSetter());

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithVoidMethod_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass")
                               .WithMethod("TestMethod", m => m
                                    .WithBody(b => b
                                        .AddStatement(@"Console.WriteLine(""Hello World!"")")));

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ClassWithTypedMethod_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass")
                               .WithMethod("TestMethod", m => m
                                    .WithBody(b => b
                                        .AddStatement(@"Console.WriteLine(GetMessage());")))
                               .WithMethod<string>("GetMessage", m => m
                                    .WithBody(b => b
                                        .AddStatement(@"return ""Hello World!"";")));

        await BuildAndVerify(classBuilder);
    }

    [Fact]
    public async Task Create_ComplexClass_Verified()
    {
        var classBuilder = new ClassBuilder("TestClass")
                                    .InheritsFrom<SomeClass>()
                                    .ImplementsInterface<ISomeInterface>()
                               .WithAttributes(a => a
                                    .AddAttribute<JsonSerializableAttribute>("typeof(TestMethod)")
                                    .AddAttribute<AnotherAttribute>())
                               .WithProperty<string>("Message", p => p.WithGetter().WithSetter())
                               .WithConstructor(c => c.Parameter<string>("defaultMessage")
                                    .WithBody(b => b
                                        .AddStatement("Message = defaultMessage;")))
                               .WithMethod("TestMethod", m => m
                                    .WithBody(b => b
                                        .AddStatement(@"Console.WriteLine(Message);")))
                               .WithMethod("SetMessage", m => m
                                    .Parameter<string>("message")
                                    .WithBody(b => b
                                        .AddStatement(@"Message = message;")));

        await BuildAndVerify(classBuilder);
    }

    private static async Task BuildAndVerify(ClassBuilder classBuilder)
    {
        var generatedClass = classBuilder.Build();

        // Verify the generated class
        await Verify(generatedClass.NormalizeWhitespace().ToFullString());
    }
}
