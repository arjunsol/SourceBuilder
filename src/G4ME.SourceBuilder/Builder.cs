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
            var classBuilder = new ClassBuilder(className, _namespace);
            configure(classBuilder);
            _typeBuilders.Add(classBuilder);
            return this;
        }

        public Builder AddInterface(string interfaceName, Action<InterfaceBuilder> configure)
        {
            var interfaceBuilder = new InterfaceBuilder(interfaceName, _namespace);
            configure(interfaceBuilder);
            _typeBuilders.Add(interfaceBuilder);
            return this;
        }

        public Builder AddRecord(string recordName, Action<RecordBuilder> configure)
        {
            var recordBuilder = new RecordBuilder(recordName, _namespace);
            configure(recordBuilder);
            _typeBuilders.Add(recordBuilder);
            return this;
        }

        private CompilationUnitSyntax BuildCompilationUnit()
        {
            var compilationBuilder = new CompilationUnitBuilder([.. _typeBuilders]);
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
