using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class AzureFunctionClassDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual IEnumerable<string> GetClassEntryDefinitionList() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> GetConstructorParameterDefinitionList() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> GetConstructorBodyStatementList() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> GetRunMethodParameterDefinitionList() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> GetRunMethodEntryStatementList() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> GetRunMethodBodyStatementList() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> GetRunMethodExitStatementList() => Enumerable.Empty<string>();

        public virtual IEnumerable<ExceptionCatchBlock> GetExceptionCatchBlocks() =>
            Enumerable.Empty<ExceptionCatchBlock>();

        public virtual IEnumerable<INugetPackageInfo> GetNugetDependencies() => Enumerable.Empty<INugetPackageInfo>();
    }

    [IntentManaged(Mode.Ignore)]
    public class ExceptionCatchBlock
    {
        private readonly List<string> _requiredNamespaces = new();
        private readonly List<string> _statementLines = new();

        public ExceptionCatchBlock(string exceptionType)
        {
            ExceptionType = exceptionType ?? throw new ArgumentException(nameof(exceptionType));
        }

        public ExceptionCatchBlock AddNamespaces(params string[] namespaces)
        {
            _requiredNamespaces.AddRange(namespaces);
            return this;
        }

        public ExceptionCatchBlock AddStatementLines(params string[] lines)
        {
            _statementLines.AddRange(lines);
            return this;
        }

        public string ExceptionType { get; }
        public IEnumerable<string> RequiredNamespaces => _requiredNamespaces;
        public IEnumerable<string> StatementLines => _statementLines;
    }
}