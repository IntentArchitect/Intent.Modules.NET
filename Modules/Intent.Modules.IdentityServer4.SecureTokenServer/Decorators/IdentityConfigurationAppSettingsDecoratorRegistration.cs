using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    public class IdentityConfigurationAppSettingsDecoratorRegistration : DecoratorRegistration<AppSettingsDecorator>
    {
        public override string DecoratorId => IdentityConfigurationAppSettingsDecorator.Identifier;

        public override AppSettingsDecorator CreateDecoratorInstance(IApplication application)
        {
            return new IdentityConfigurationAppSettingsDecorator(application);
        }
    }
}
