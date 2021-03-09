using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.Security.BearerToken.Decorators
{
    public class BearerTokenConfigurationSettingsDecoratorRegistration : DecoratorRegistration<AppSettingsDecorator>
    {
        public override string DecoratorId => BearerTokenConfigurationSettingsDecorator.DecoratorId;

        public override AppSettingsDecorator CreateDecoratorInstance(IApplication application)
        {
            return new BearerTokenConfigurationSettingsDecorator(application);
        }
    }
}
