using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.Events;
using Intent.Modules.Common;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.STS.Interop.Swashbuckle.JWT.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class InteropSwashbuckleAppSettingsDecorator : AppSettingsDecorator, IDecoratorExecutionHooks
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.STS.Interop.Swashbuckle.JWT.InteropSwashbuckleAppSettingsDecorator";

        private readonly AppSettingsTemplate _template;
        private readonly IApplication _application;
        private string _stsPort = "{sts_port}";

        [IntentManaged(Mode.Merge)]
        public InteropSwashbuckleAppSettingsDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _application.EventDispatcher.Subscribe<SecureTokenServiceHostedEvent>(Handle); // This is just temporary. Need to store these settings in a solution-wide accessible space for each app.
        }

        private void Handle(SecureTokenServiceHostedEvent @event)
        {
            _stsPort = @event.Port;
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new SwaggerOAuth2SchemeEvent(
                schemeName: "Password",
                priority: 10,
                clientId: "ResourceOwner_Client",
                authUrl: $"https://localhost:{_stsPort}/connect/authorize",
                tokenUrl: $"https://localhost:{_stsPort}/connect/token",
                scopes: new Dictionary<string, string> { { "roles", "Roles Scope" }, { "api", "API Scope" } }));
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {

        }
    }
}