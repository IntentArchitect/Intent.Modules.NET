using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Templates.ValidationBehaviour
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class ValidationBehaviourContract : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual IEnumerable<string> GetGenericTypeParameters() => Enumerable.Empty<string>();

        public virtual IEnumerable<string> GetInheritanceDeclarations() => Enumerable.Empty<string>();

        public virtual IEnumerable<string> GetGenericTypeConstraints() => Enumerable.Empty<string>();

        public virtual string GetHandleReturnType() => null;

        public virtual IEnumerable<string> GetHandleParameterList() => Enumerable.Empty<string>();

        public virtual IEnumerable<string> GetHandleExitStatementList() => Enumerable.Empty<string>();
    }
}