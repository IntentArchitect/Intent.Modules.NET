using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.Security.BearerToken.Decorators
{
    public class BearerTokenConfigurationSettingsDecorator : AppSettingsDecorator
    {
        public const string DecoratorId = "Application.Security.BearerTokenConfigurationSettingsDecorator";

        private readonly IApplication _application;

        public BearerTokenConfigurationSettingsDecorator(IApplication application)
        {
            _application = application;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            appSettings.AddPropertyIfNotExists("Security.Bearer", new
            {
                Authority = "https://stshost.here",
                Audience = "api"
            });
        }
    }
}
