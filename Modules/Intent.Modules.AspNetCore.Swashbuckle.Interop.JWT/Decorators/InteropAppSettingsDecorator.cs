using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.Common.Configuration;
using Intent.Modules.Common.CSharp.Configuration;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class InteropAppSettingsDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Swashbuckle.Interop.JWT.InteropAppSettingsDecorator";

        private readonly AppSettingsTemplate _template;
        private readonly IApplication _application;
        private string _stsPort = "{sts_port}";

        [IntentManaged(Mode.Merge)]
        public InteropAppSettingsDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _application.EventDispatcher.Subscribe<SecureTokenServiceHostedEvent>(Handle); // This is just temporary. Need to store these settings in a solution-wide accessible space for each app.
        }

        private void Handle(SecureTokenServiceHostedEvent @event)
        {
            _stsPort = @event.Port;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            var settings = appSettings.GetProperty("Swashbuckle");

            if (settings.SwaggerGen.SwaggerGeneratorOptions.SecuritySchemes == null)
            {
                settings.SwaggerGen.SwaggerGeneratorOptions.SecuritySchemes = JObject.FromObject(new
                {
                    oauth2 = new
                    {
                        Type = "OAuth2",
                        Flows = new
                        {
                            Password = new
                            {
                                AuthorizationUrl = $"https://localhost:{_stsPort}/connect/authorize",
                                TokenUrl = $"https://localhost:{_stsPort}/connect/token",
                                Scopes = new
                                {
                                    roles = "Roles scope",
                                    api = "API scope"
                                }
                            },
                            AuthorizationCode = new
                            {
                                AuthorizationUrl = $"https://localhost:{_stsPort}/connect/authorize",
                                TokenUrl = $"https://localhost:{_stsPort}/connect/token",
                                RefreshUrl = (string)null,
                                Scopes = new
                                {
                                    roles = "Roles scope",
                                    api = "API scope"
                                }
                            }
                        }
                    }
                });
            }

            if (settings.SwaggerUI.OAuthConfigObject == null)
            {
                settings.SwaggerUI.OAuthConfigObject = JObject.FromObject(new
                {
                    ClientId = "ResourceOwner_Client / Auth_Code_Client",
                    ClientSecret = (string)null,
                    AppName = _template.OutputTarget.Application.Name,
                    UsePkceWithAuthorizationCodeGrant = true,
                    ScopeSeparator = " "
                });
            }
        }
    }
}