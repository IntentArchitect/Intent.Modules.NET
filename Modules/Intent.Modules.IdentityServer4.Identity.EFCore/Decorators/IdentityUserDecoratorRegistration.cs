using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.Decorators
{
    [Description(IdentityUserDecorator.DecoratorId)]
    public class IdentityUserDecoratorRegistration : DecoratorRegistration<IdentityServerConfigurationTemplate, IdentityConfigurationDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public override IdentityConfigurationDecorator CreateDecoratorInstance(IdentityServerConfigurationTemplate template, IApplication application)
        {
            return new IdentityUserDecorator(template, application);
        }

        public override string DecoratorId => IdentityUserDecorator.DecoratorId;
    }
}