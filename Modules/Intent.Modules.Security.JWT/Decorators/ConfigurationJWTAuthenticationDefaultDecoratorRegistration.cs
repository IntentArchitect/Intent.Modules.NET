using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Security.JWT.Templates.ConfigurationJWTAuthentication;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Security.JWT.Decorators
{
    [Description(ConfigurationJWTAuthenticationDefaultDecorator.DecoratorId)]
    public class ConfigurationJWTAuthenticationDefaultDecoratorRegistration : DecoratorRegistration<ConfigurationJWTAuthenticationTemplate, JWTAuthenticationDecorator>
    {
        public override JWTAuthenticationDecorator CreateDecoratorInstance(ConfigurationJWTAuthenticationTemplate template, IApplication application)
        {
            return new ConfigurationJWTAuthenticationDefaultDecorator(template, application);
        }

        public override string DecoratorId => ConfigurationJWTAuthenticationDefaultDecorator.DecoratorId;
    }
}