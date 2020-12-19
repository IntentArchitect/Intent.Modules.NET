using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Interop.Contracts.Decorators
{
    [Description(ContractDispatchDecorator.DecoratorId)]
    public class ContractDispatchDecoratorRegistration : DecoratorRegistration<ControllerTemplate, ControllerDecorator>
    {
        public override ControllerDecorator CreateDecoratorInstance(ControllerTemplate template, IApplication application)
        {
            return new ContractDispatchDecorator(template);
        }

        public override string DecoratorId => ContractDispatchDecorator.DecoratorId;
    }
}