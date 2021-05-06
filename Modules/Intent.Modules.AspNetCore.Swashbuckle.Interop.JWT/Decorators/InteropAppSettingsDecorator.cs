using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

        [IntentManaged(Mode.Merge)]
        public InteropAppSettingsDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            var settings = appSettings.GetPropertyAs<Dictionary<string, object>>("Swashbuckle");

            var schemes = new
            {
                oauth2 = new
                {
                    Type = "OAuth2",
                    Flows = new
                    {
                        Password = new
                        {
                            AuthorizationUrl = "https://localhost:{sts port}/connect/authorize",
                            TokenUrl = "https://localhost:{sts port}/connect/token",
                            Scopes = new
                            {
                                roles = "Roles scope",
                                api = "API scope"
                            }
                        },
                        AuthorizationCode = new
                        {
                            AuthorizationUrl = "https://localhost:{sts port}/connect/authorize",
                            TokenUrl = "https://localhost:{sts port}/connect/token",
                            RefreshUrl = (string)null,
                            Scopes = new
                            {
                                roles = "Roles scope",
                                api = "API scope"
                            }
                        }
                    }
                }
            };
            settings["SwaggerGen"] = new Dictionary<string, object> { { "SecuritySchemes", schemes } };

            settings["SwaggerUI"] = new Dictionary<string, object>
            {
                { "RoutePrefix", " " },
                {
                    "OAuthConfigObject", new
                    {
                        ClientId = "ResourceOwner_Client / Auth_Code_Client",
                        ClientSecret = (string)null,
                        AppName = "NewApplication",
                        UsePkceWithAuthorizationCodeGrant = true,
                        ScopeSeparator = " "
                    }
                }
            };

            appSettings.SetProperty("Swashbuckle", settings);
        }
    }
}