using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    public abstract class ServiceContractDecorator : ITemplateDecorator
    {
        public virtual int Priority => 0;
        // public virtual string ContractAttributes(ServiceModel service) => null;
        // public virtual string OperationAttributes(ServiceModel service, OperationModel operation) => null;
        // public virtual string EnterClass() => null;
        // public virtual string ExitClass() => null;
    }
}
