using G4ME.SourceBuilder.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace G4ME.SourceBuilder;

// TODO: Builder should take an options class override to enable or disable some features
public class Builder(string sharedNamespace)
{
    // TODO: Create a collections class that adds validation for unique names and such
    private readonly List<ITypeBuilder> _typeBuilders = [ ];

    public Builder AddClass(string className, Action<ClassBuilder> configure)
    {
        ClassBuilder classBuilder = new(className, sharedNamespace);
        
        configure(classBuilder);
        _typeBuilders.Add(classBuilder);
        
        return this;
    }

    public Builder AddInterface(string interfaceName, Action<InterfaceBuilder> configure)
    {
        InterfaceBuilder interfaceBuilder = new(interfaceName, sharedNamespace);
        
        configure(interfaceBuilder);
        _typeBuilders.Add(interfaceBuilder);
        
        return this;
    }

    public Builder AddRecord(string recordName, Action<RecordBuilder> configure)
    {
        RecordBuilder recordBuilder = new(recordName, sharedNamespace);
                    
        configure(recordBuilder);
        _typeBuilders.Add(recordBuilder);
        
        return this;
    }

    private CompilationUnitSyntax BuildCompilationUnit()
    {
        CompilationUnitBuilder compilationBuilder = new([.. _typeBuilders]);
        
        return compilationBuilder.Build();
    }

    public CompilationUnitSyntax Build() => BuildCompilationUnit();

    public override string ToString()
    {
        var compilationUnit = BuildCompilationUnit();
        
        return compilationUnit.NormalizeWhitespace().ToFullString();
    }
}
