using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Identity.EntityFrameworkCore.Decorators
{
    [Description(AspNetIdentityUserDecorator.DecoratorId)]
    public class AspNetIdentityUserDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new AspNetIdentityUserDecorator(template, application);
        }

        public override string DecoratorId => AspNetIdentityUserDecorator.DecoratorId;
    }
}