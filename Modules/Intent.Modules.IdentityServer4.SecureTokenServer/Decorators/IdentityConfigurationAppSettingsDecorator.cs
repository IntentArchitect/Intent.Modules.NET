using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    public class IdentityConfigurationAppSettingsDecorator : AppSettingsDecorator
    {
        public const string Identifier = "IdentityServer4.SecureTokenServer.IdentityConfigurationAppSettingsDecorator";
        private readonly string _appName;

        public IdentityConfigurationAppSettingsDecorator(IApplication application)
        {
            _appName = application.Name;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            appSettings.AddPropertyIfNotExists("IdentityServer", new
            {
                Clients = new[]
                {
                    new
                    {
                        Enabled = true,
                        ClientId = $"{_appName}_client",
                        ClientName = $"{_appName} Client",
                        RequireClientSecret = false,
                        AllowedGrantTypes = new []{ "authorization_code" },
                        AllowedScopes = new []{ "api", "roles", "openid", "profile", "email" },
                        RequirePkce = true,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = new []{ "https://localhost/oauth2-redirect.html" },
                        PostLogoutRedirectUris = new string[0],
                        AllowPlainTextPkce = false,
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
