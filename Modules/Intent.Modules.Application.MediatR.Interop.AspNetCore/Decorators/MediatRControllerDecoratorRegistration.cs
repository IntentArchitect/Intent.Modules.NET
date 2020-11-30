using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Interop.AspNetCore.Decorators
{
    [Description(MediatRControllerDecorator.DecoratorId)]
    public class MediatRControllerDecoratorRegistration : DecoratorRegistration<ControllerTemplate, ControllerDecorator>
    {
        public override ControllerDecorator CreateDecoratorInstance(ControllerTemplate template, IApplication application)
        {
            return new MediatRControllerDecorator(template);
        }

        public override string DecoratorId => MediatRControllerDecorator.DecoratorId;
    }
}