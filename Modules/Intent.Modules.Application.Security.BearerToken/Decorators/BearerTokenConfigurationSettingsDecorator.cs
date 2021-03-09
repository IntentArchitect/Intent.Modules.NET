using Intent.Engine;
using Intent.Modules.Application.Security.BearerToken.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.Security.BearerToken.Decorators
{
    public class BearerTokenConfigurationSettingsDecorator : AppSettingsDecorator
    {
        public const string DecoratorId = "Application.Security.BearerToken.BearerTokenConfigurationSettingsDecorator";

        private readonly IApplication _application;
        private bool _overrideBearerTokenConfiguration;

        public BearerTokenConfigurationSettingsDecorator(IApplication application)
        {
            _application = application;
            application.EventDispatcher.Subscribe<OverrideBearerTokenConfigurationEvent>(evt =>
            {
                _overrideBearerTokenConfiguration = true;
            });
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            if (_overrideBearerTokenConfiguration) { return; }

            appSettings.AddPropertyIfNotExists("Security.Bearer", new
            {
                Authority = "https://stshost.here",
                Audience = "api"
            });
        }
    }
}
