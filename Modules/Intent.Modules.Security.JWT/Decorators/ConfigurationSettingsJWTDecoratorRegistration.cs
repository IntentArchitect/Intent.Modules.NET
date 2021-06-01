using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Security.JWT.Decorators
{

    [Description(ConfigurationSettingsJWTDecorator.DecoratorId)]
    public class ConfigurationSettingsJWTDecoratorRegistration : DecoratorRegistration<AppSettingsTemplate, AppSettingsDecorator>
    {
        public override AppSettingsDecorator CreateDecoratorInstance(AppSettingsTemplate template, IApplication application)
        {
            return new ConfigurationSettingsJWTDecorator(template, application);
        }

        public override string DecoratorId => ConfigurationSettingsJWTDecorator.DecoratorId;
    }
}
