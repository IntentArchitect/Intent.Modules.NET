using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [Description(IdentityConfigurationAppSettingsDecorator.DecoratorId)]
    public class IdentityConfigurationAppSettingsDecoratorRegistration : DecoratorRegistration<AppSettingsTemplate, AppSettingsDecorator>
    {
        public override AppSettingsDecorator CreateDecoratorInstance(AppSettingsTemplate template, IApplication application)
        {
            return new IdentityConfigurationAppSettingsDecorator(template, application);
        }

        public override string DecoratorId => IdentityConfigurationAppSettingsDecorator.DecoratorId;
    }
}
