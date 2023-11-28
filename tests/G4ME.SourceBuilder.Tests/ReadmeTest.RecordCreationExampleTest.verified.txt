using G4ME.SourceBuilder.Tests;
using System;

namespace ExampleNamespace;
public record PersonRequest(string Name) : IRequest<PersonResponse>;