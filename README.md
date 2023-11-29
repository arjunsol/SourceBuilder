# G4ME SourceBuilder

## Overview
G4ME SourceBuilder is a dynamic .NET code generation framework for programmable construction of classes, records, and interfaces, optimised for Visual Studio 2022 and .NET 5+ environments.

## Features

- **Builder**: Central orchestrator for adding types within a shared namespace.
- **CompilationUnitBuilder**: Combine various type builders into a cohesive compilation unit.
- **ClassBuilder**: Create and configure classes with inheritance, interfaces, and attributes.
- **RecordBuilder**: Construct record types with parameters and attributes.
- **InterfaceBuilder**: Build interfaces with methods, properties, and attributes.

## Usage

### Creating a class with method

```csharp
new Builder("ExampleNamespace")
                .AddClass("Person", c => c
                    .Properties(p => p
                        .Add<string>("Name").Get().PrivateSet())
                    .Constructor(c => c
                        .Parameter<string>("name")
                        .Body("Name = name;"))
                    .AddMethod("Greet", m => m
                        .Body(@"Console.WriteLine($"Hello, {Name}!");")))
                    .ToString();
```

#### Result:

```csharp
using System;

namespace ExampleNamespace;
public class Person
{
    public string Name { get; private set; }

    public Person(string name)
    {
        Name = name;
    }

    public void Greet()
    {
        Console.WriteLine($"Hello, {Name}!");
    }
}
```


### Creating a record with interface implementation

```csharp
new Builder("ExampleNamespace")
                .AddRecord("PersonRequest", r => r.Implements<IRequest<PersonResponse>>()
                    .Parameter<string>("Name"))
                    .ToString();
```

#### Result:

```csharp
using G4ME.SourceBuilder.Tests.Verified;
using System;

namespace ExampleNamespace;
public record PersonRequest(string Name) : IRequest<PersonResponse>;
```


### Creating a basic interface with single property

```csharp
new Builder("Example")
                .AddInterface("IResponse", i => i
                    .Properties(p => p
                        .Add<string>("Name").Get()))
                .ToString();
#### Result:

```csharp
namespace Example;
public interface IResponse
{
    public string Name { get; }
}
```


