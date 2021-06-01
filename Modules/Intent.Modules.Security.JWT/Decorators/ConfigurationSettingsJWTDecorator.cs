using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.AspNetCore.Events;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Security.JWT.Events;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Security.JWT.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConfigurationSettingsJWTDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.JWT.ConfigurationSettingsJWTDecorator";

        private readonly AppSettingsTemplate _template;
        private readonly IApplication _application;
        private bool _stsIsSelfHostedInThisApplication;
        private string _stsPort = "{sts_port}";

        [IntentManaged(Mode.Merge)]
        public ConfigurationSettingsJWTDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            application.EventDispatcher.Subscribe<OverrideBearerTokenConfigurationEvent>(evt =>
            {
                _stsIsSelfHostedInThisApplication = true;
            });
            _application.EventDispatcher.Subscribe<SecureTokenServiceHostedEvent>(Handle); // This is just temporary. Need to store these settings in a solution-wide accessible space for each app.
        }

        private void Handle(SecureTokenServiceHostedEvent @event)
        {
            _stsPort = @event.Port;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            if (_stsIsSelfHostedInThisApplication) { return; }

            appSettings.AddPropertyIfNotExists("Security.Bearer", new
            {
                Authority = $"https://localhost:{_stsPort}",
                Audience = "api"
            });
        }
    }
}
