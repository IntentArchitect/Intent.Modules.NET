using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class ServiceImplementationDecorator : ITemplateDecorator
    {
        public virtual IEnumerable<ConstructorParameter> GetConstructorDependencies()
        {
            return new List<ConstructorParameter>();
        }

        public virtual string GetDecoratedImplementation(OperationModel operationModel)
        {
            return string.Empty;
        }

        public int Priority { get; protected set; } = 0;
    }
}
