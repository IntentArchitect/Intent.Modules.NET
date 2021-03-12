using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Decorators
{
    [Description(MVCControllerEndpointStartupDecorator.DecoratorId)]
    public class MVCControllerEndpointStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new MVCControllerEndpointStartupDecorator(template, application);
        }

        public override string DecoratorId => MVCControllerEndpointStartupDecorator.DecoratorId;
    }
}