using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.Decorators
{
    [Description(ConfigurationSettingsMSALDecorator.DecoratorId)]
    public class ConfigurationSettingsMSALDecoratorRegistration : DecoratorRegistration<AppSettingsTemplate, AppSettingsDecorator>
    {
        public override AppSettingsDecorator CreateDecoratorInstance(AppSettingsTemplate template, IApplication application)
        {
            return new ConfigurationSettingsMSALDecorator(template, application);
        }

        public override string DecoratorId => ConfigurationSettingsMSALDecorator.DecoratorId;
    }
}