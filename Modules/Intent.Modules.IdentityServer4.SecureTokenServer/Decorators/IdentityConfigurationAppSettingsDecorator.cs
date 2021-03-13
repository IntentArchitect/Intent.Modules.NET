using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityConfigurationAppSettingsDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.SecureTokenServer.IdentityConfigurationAppSettingsDecorator";

        private readonly AppSettingsTemplate _template;
        private readonly IApplication _application;
        private readonly string _appName;

        public IdentityConfigurationAppSettingsDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _appName = application.Name;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            appSettings.AddPropertyIfNotExists("IdentityServer", new
            {
                Clients = new object[]
                {
                    new
                    {
                        Enabled = true,
                        ClientId = $"{_appName}_ResourceOwner_Client",
                        ClientName = $"{_appName} Resource-Owner Client",
                        RequireClientSecret = false,
                        ClientSecrets = new []{ new { Value = "secret" } },
                        AllowedGrantTypes = new []{ "password" },
                        AllowedScopes = new []{ "api", "roles", "openid", "profile", "email" }
                    },
                    new
                    {
                        Enabled = false,
                        ClientId = $"{_appName}_AuthCode_Client",
                        ClientName = $"{_appName} Authorize-Code Client",
                        RequireClientSecret = false,
                        ClientSecrets = new []{ new { Value = "secret" } },
                        AllowedGrantTypes = new []{ "authorization_code" },
                        AllowedScopes = new []{ "api", "roles", "openid", "profile", "email" },
                        RequirePkce = true,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = new string[]
                        {
                            "https://my-app-domain/oauth2-redirect.html"
                        },
                        PostLogoutRedirectUris = new string[]
                        {
                            "https://my-app-domain/logout.html"
                        },
                        AllowPlainTextPkce = false
                    }
                },
                ApiScopes = new[]
                {
                    new
                    {
                        Name = "api",
                        UserClaims = (string[])null
                    },
                    new
                    {
                        Name = "roles",
                        UserClaims = new[]{ "role" }
                    }
                },
                ApiResources = new[]
                {
                    new
                    {
                        Name ="api",
                        Scopes = new[]{ "api", "roles" }
                    }
                },
                IdentityResources = new[]
                {
                    new
                    {
                        Name = "openid"
                    },
                    new
                    {
                        Name = "profile"
                    },
                    new
                    {
                        Name = "email"
                    }
                }
            });
        }
    }
}
