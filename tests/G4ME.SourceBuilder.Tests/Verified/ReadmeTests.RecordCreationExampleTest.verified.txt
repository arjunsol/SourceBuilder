using G4ME.SourceBuilder.Tests.Verified;
using System;

namespace ExampleNamespace;
public record PersonRequest(string Name) : IRequest<PersonResponse>;