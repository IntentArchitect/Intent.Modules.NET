using System.Collections.Generic;
using System.Linq;
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
    }
}