using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Security.MSAL.Templates.ConfigurationMSALAuthentication;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.Decorators
{
    [Description(ConfigurationMSALAuthenticationDefaultDecorator.DecoratorId)]
    public class ConfigurationMSALAuthenticationDefaultDecoratorRegistration : DecoratorRegistration<ConfigurationMSALAuthenticationTemplate, MSALAuthenticationDecorator>
    {
        public override MSALAuthenticationDecorator CreateDecoratorInstance(ConfigurationMSALAuthenticationTemplate template, IApplication application)
        {
            return new ConfigurationMSALAuthenticationDefaultDecorator(template, application);
        }

        public override string DecoratorId => ConfigurationMSALAuthenticationDefaultDecorator.DecoratorId;
    }
}