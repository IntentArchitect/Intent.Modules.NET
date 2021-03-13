using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Security.JWT.Events;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Security.JWT.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class JWTConfigurationSettingsDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.JWT.JWTConfigurationSettingsDecorator";

        private readonly AppSettingsTemplate _template;
        private readonly IApplication _application;
        private bool _overrideBearerTokenConfiguration;

        public JWTConfigurationSettingsDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
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
