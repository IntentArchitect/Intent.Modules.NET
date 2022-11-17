using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.Events;
using Intent.Modules.Common;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Interop.Swashbuckle.JWT.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class InteropSwashbuckleAppSettingsDecorator : AppSettingsDecorator, IDecoratorExecutionHooks
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.Interop.Swashbuckle.JWT.InteropSwashbuckleAppSettingsDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AppSettingsTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public InteropSwashbuckleAppSettingsDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new SwaggerOAuth2SchemeEvent(
                schemeName: "AuthorizationCode",
                priority: 20,
                clientId: "Auth_Code_Client",
                authUrl: $"https://localhost:{SchemeEventConstants.STS_Port_Tag}/connect/authorize",
                tokenUrl: $"https://localhost:{SchemeEventConstants.STS_Port_Tag}/connect/token",
                refreshUrl: null,
                scopes: new Dictionary<string, string> { { "api", "API Scope" } }));
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {

        }
    }
}