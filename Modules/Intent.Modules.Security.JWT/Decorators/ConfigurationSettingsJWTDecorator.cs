using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Security.JWT.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConfigurationSettingsJWTDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.JWT.ConfigurationSettingsJWTDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AppSettingsTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        private string _stsPort = "{sts_port}";

        [IntentManaged(Mode.Merge)]
        public ConfigurationSettingsJWTDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _application.EventDispatcher.Subscribe<SecureTokenServiceHostedEvent>(Handle); // This is just temporary. Need to store these settings in a solution-wide accessible space for each app.

            _template.ApplyAppSetting("Security.Bearer", new
            {
                Authority = "https://localhost:{sts_port}",
                Audience = "api"
            });
        }

        private void Handle(SecureTokenServiceHostedEvent @event)
        {
            _stsPort = @event.Port;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            if (((string)appSettings.GetProperty("Security.Bearer")?["Authority"])?.Contains("{sts_port}") == true && _stsPort != "{sts_port}")
            {
                appSettings.GetProperty("Security.Bearer")["Authority"] = $"https://localhost:{_stsPort}";
            }
        }
    }
}
