using System;
using G4ME.SourceBuilder.Tests.Verified;

namespace ExampleNamespace;
public record PersonRequest(string Name) : IRequest<PersonResponse>;