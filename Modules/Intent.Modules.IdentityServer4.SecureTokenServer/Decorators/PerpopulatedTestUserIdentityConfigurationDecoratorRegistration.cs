using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [Description(PerpopulatedTestUserIdentityConfigurationDecorator.DecoratorId)]
    public class PerpopulatedTestUserIdentityConfigurationDecoratorRegistration : DecoratorRegistration<IdentityServerConfigurationTemplate, IdentityConfigurationDecorator>
    {
        public override IdentityConfigurationDecorator CreateDecoratorInstance(IdentityServerConfigurationTemplate template, IApplication application)
        {
            return new PerpopulatedTestUserIdentityConfigurationDecorator(template, application);
        }

        public override string DecoratorId => PerpopulatedTestUserIdentityConfigurationDecorator.DecoratorId;
    }
}