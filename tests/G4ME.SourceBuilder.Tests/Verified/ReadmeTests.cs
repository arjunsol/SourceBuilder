using System.Reflection;
using System.Text;

namespace G4ME.SourceBuilder.Tests.Verified;

[UsesVerify]
[TestCaseOrderer(
    ordererTypeName: "G4ME.SourceBuilder.Tests.PriorityOrderer",
    ordererAssemblyName: "G4ME.SourceBuilder.Tests")]
public class ReadmeTests
{
    private static StringBuilder _readme = new();

    [Fact, TestPriority(1)]
    public async Task ClassCreationExampleTest()
    {
        string generatedCode = ClassExample();

        await Verify(generatedCode);

        BuildReadmeHeader();

        _readme.AppendLine("### Creating a class with method\r\n");

        _readme.AppendLine("```csharp");
        _readme.AppendLine(@"new Builder(""ExampleNamespace"")
                .AddClass(""Person"", c => c
                    .Properties(p => p
                        .Add<string>(""Name"").Get().PrivateSet())
                    .Constructor(c => c
                        .Parameter<string>(""name"")
                        .Body(""Name = name;""))
                    .AddMethod(""Greet"", m => m
                        .Body(@""Console.WriteLine($""Hello, {Name}!"");"")))
                    .ToString();");
        _readme.AppendLine("```\r\n");

        WrapAndWriteCode(generatedCode);
    }

    [Fact, TestPriority(2)]
    public async Task RecordCreationExampleTest()
    {
        string generatedCode = RecordExample();

        await Verify(generatedCode);

        _readme.AppendLine("### Creating a record with interface implementation\r\n");

        _readme.AppendLine("```csharp");
        _readme.AppendLine(@"new Builder(""ExampleNamespace"")
                .AddRecord(""PersonRequest"", r => r.Implements<IRequest<PersonResponse>>()
                    .Parameter<string>(""Name""))
                    .ToString();");
        _readme.AppendLine("```\r\n");

        WrapAndWriteCode(generatedCode);
    }

    [Fact, TestPriority(3)]
    public async Task InterfaceCreationExample()
    {
        string generatedCode = InterfaceExample();

        await Verify(generatedCode);

        _readme.AppendLine("### Creating a basic interface with single property\r\n");

        _readme.AppendLine("```csharp");
        _readme.AppendLine(@"new Builder(""Example"")
                .AddInterface(""IResponse"", i => i
                    .Properties(p => p
                        .Add<string>(""Name"").Get()))
                .ToString();");

        WrapAndWriteCode(generatedCode);
    }

    [Fact, TestPriority(99)]
    public async Task Finalise()
    {
        string solutionRoot = FindSolutionRoot();
        string readmeFile = "README.md";
        string path = Path.Combine(solutionRoot, readmeFile);

        await SaveReadmeToFile(path);
    }

    private void WrapAndWriteCode(string generatedCode)
    {
        _readme.AppendLine("#### Result:\r\n");
        _readme.AppendLine("```csharp");
        _readme.AppendLine(generatedCode);
        _readme.AppendLine("```");
        _readme.AppendLine("\r\n");
    }

    private void BuildReadmeHeader()
    {
        _readme.AppendLine("# G4ME SourceBuilder\r\n");
        _readme.AppendLine("## Overview");
        _readme.AppendLine("G4ME SourceBuilder is a dynamic .NET code generation framework for programmable construction of classes, records, and interfaces, optimised for Visual Studio 2022 and .NET 5+ environments.\r\n");
        _readme.AppendLine("## Features\r\n");
        _readme.AppendLine("- **Builder**: Central orchestrator for adding types within a shared namespace.");
        _readme.AppendLine("- **CompilationUnitBuilder**: Combine various type builders into a cohesive compilation unit.");
        _readme.AppendLine("- **ClassBuilder**: Create and configure classes with inheritance, interfaces, and attributes.");
        _readme.AppendLine("- **RecordBuilder**: Construct record types with parameters and attributes.");
        _readme.AppendLine("- **InterfaceBuilder**: Build interfaces with methods, properties, and attributes.\r\n");
        _readme.AppendLine("## Usage\r\n");
    }

    private static string ClassExample() =>
        new Builder("ExampleNamespace")
                .AddClass("Person", c => c
                .Properties(p => p
                    .Add<string>("Name").Get().PrivateSet())
                .Constructor(c => c
                    .Parameter<string>("name")
                    .Body("Name = name;"))
                .AddMethod("Greet", m => m
                    .Body(@"Console.WriteLine($""Hello, {Name}!"");")))
                .ToString();

    private static string RecordExample() =>
        new Builder("ExampleNamespace")
                .AddRecord("PersonRequest", r => r
                    .Parameter<string>("Name")
                    .Implements<IRequest<PersonResponse>>())
                    .ToString();

    private static string InterfaceExample() =>
        new Builder("Example")
                .AddInterface("IResponse", i => i
                    .Properties(p => p
                        .Add<string>("Name").Get()))
                .ToString();

    private async Task SaveReadmeToFile(string filePath)
    {
        string finalReadmeContent = _readme.ToString();
        await File.WriteAllTextAsync(filePath, finalReadmeContent);
    }

    private static string FindSolutionRoot()
    {
        string assemblyPath = Assembly.GetExecutingAssembly().Location;
        string? executingAssemblyPath = Path.GetDirectoryName(assemblyPath);

        if (string.IsNullOrWhiteSpace(executingAssemblyPath))
        {
            Assert.Fail("Executing assembly path not found");
        }

        DirectoryInfo? directory = new(executingAssemblyPath);

        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "SourceBuilder.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? string.Empty;
    }

    private static string? GetReadmeFilePath()
    {
        var solutionRoot = FindSolutionRoot();
        return solutionRoot != null ? Path.Combine(solutionRoot, "README.md") : null;
    }

    private interface IRequest<T> { }

    private class PersonResponse { }
}
