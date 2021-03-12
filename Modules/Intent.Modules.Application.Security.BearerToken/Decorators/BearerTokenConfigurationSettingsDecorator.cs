using Intent.Engine;
using Intent.Modules.Application.Security.BearerToken.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Application.Security.BearerToken.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class BearerTokenConfigurationSettingsDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.Security.BearerToken.BearerTokenConfigurationSettingsDecorator";

        private readonly AppSettingsTemplate _template;
        private readonly IApplication _application;
        private bool _overrideBearerTokenConfiguration;

        public BearerTokenConfigurationSettingsDecorator(AppSettingsTemplate template, IApplication application)
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
