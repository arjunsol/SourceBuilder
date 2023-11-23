using G4ME.SourceBuilder.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace G4ME.SourceBuilder
{
    public class Builder
    {
        private readonly List<ITypeBuilder> _typeBuilders = [ ];
        private string _namespace = string.Empty;

        public Builder SetNamespace(string namespaceName)
        {
            _namespace = namespaceName;
            return this;
        }

        public Builder AddClass(string className, Action<ClassBuilder> configure)
        {
            ClassBuilder classBuilder = new(className, _namespace);
            configure(classBuilder);
            _typeBuilders.Add(classBuilder);
            return this;
        }

        public Builder AddInterface(string interfaceName, Action<InterfaceBuilder> configure)
        {
            InterfaceBuilder interfaceBuilder = new(interfaceName, _namespace);
            configure(interfaceBuilder);
            _typeBuilders.Add(interfaceBuilder);
            return this;
        }

        public Builder AddRecord(string recordName, Action<RecordBuilder> configure)
        {
            RecordBuilder recordBuilder = new(recordName, _namespace);
            configure(recordBuilder);
            _typeBuilders.Add(recordBuilder);
            return this;
        }

        private CompilationUnitSyntax BuildCompilationUnit()
        {
            CompilationUnitBuilder compilationBuilder = new([.. _typeBuilders]);
            return compilationBuilder.Build();
        }

        public CompilationUnitSyntax Build()
        {
            return BuildCompilationUnit();
        }

        public override string ToString()
        {
            var compilationUnit = BuildCompilationUnit();
            return compilationUnit.NormalizeWhitespace().ToFullString();
        }
    }
}
