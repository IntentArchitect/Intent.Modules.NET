using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Logging.Serilog.Decorators
{
    [Description(ConfigurationSettingsSerilogLoggingDecorator.DecoratorId)]
    public class ConfigurationSettingsSerilogLoggingDecoratorRegistration : DecoratorRegistration<AppSettingsTemplate, AppSettingsDecorator>
    {
        public override AppSettingsDecorator CreateDecoratorInstance(AppSettingsTemplate template, IApplication application)
        {
            return new ConfigurationSettingsSerilogLoggingDecorator(template, application);
        }

        public override string DecoratorId => ConfigurationSettingsSerilogLoggingDecorator.DecoratorId;
    }
}