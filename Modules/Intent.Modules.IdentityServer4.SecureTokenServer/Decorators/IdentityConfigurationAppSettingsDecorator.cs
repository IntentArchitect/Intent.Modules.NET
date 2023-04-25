using System;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.Common.Configuration;
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

        [IntentManaged(Mode.Fully)]
        private readonly AppSettingsTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;
        private readonly string _appName;
        private string _sslPort = "client_port";

        [IntentManaged(Mode.Merge)]
        public IdentityConfigurationAppSettingsDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _appName = application.Name;
            _application.EventDispatcher.Subscribe<HostingSettingsCreatedEvent>(Handle); // This is just temporary. Need to store these settings in a solution-wide accessible space for each app.
        }

        private void Handle(HostingSettingsCreatedEvent @event)
        {
            _sslPort = @event.SslPort.ToString();
            _application.EventDispatcher.Publish(new SecureTokenServiceHostedEvent(_sslPort));
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
                        ClientId = $"ResourceOwner_Client",
                        ClientName = $"{_appName} Resource-Owner Client",
                        RequireClientSecret = false,
                        ClientSecrets = new []{ new { Value = "secret" } },
                        AllowedGrantTypes = new []{ "password" },
                        AllowedScopes = new []{ "api", "openid", "profile", "email" }
                    },
                    new
                    {
                        Enabled = true,
                        ClientId = $"ClientCredential_Client",
                        ClientName = $"{_appName} Client-Credential Client",
                        Description = "See more: https://www.oauth.com/oauth2-servers/access-tokens/client-credentials/. To generate secret: https://www.liavaag.org/English/SHA-Generator/ (SHA-256 and Base-64 output). Current secret is 'secret'.",
                        RequireClientSecret = true,
                        ClientSecrets = new []{ new { Value = "K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=" } },
                        AllowedGrantTypes = new []{ "client_credentials" },
                        AllowedScopes = new []{ "api" }
                    },
                    new
                    {
                        Enabled = true,
                        ClientId = $"Auth_Code_Client",
                        ClientName = $"{_appName} Authorize-Code Client",
                        RequireClientSecret = false,
                        ClientSecrets = new []{ new { Value = "secret" } },
                        AllowedGrantTypes = new []{ "authorization_code" },
                        AllowedScopes = new []{ "api", "openid", "profile", "email" },
                        RequirePkce = true,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = new string[]
                        {
                            $"https://localhost:{_sslPort}/authentication/login-callback",
                            $"https://localhost:{_sslPort}/swagger/oauth2-redirect.html"
                        },
                        PostLogoutRedirectUris = new string[]
                        {
                            $"https://localhost:{_sslPort}/authentication/logout-callback"
                        },
                        AllowPlainTextPkce = false,
                        AllowedCorsOrigins = Array.Empty<string>()
                    }
                },
                ApiScopes = new[]
                {
                    new
                    {
                        Name = "api",
                        UserClaims = new[]{ "role", "__tenant__" }
                    }
                },
                ApiResources = new[]
                {
                    new
                    {
                        Name ="api",
                        Scopes = new[]{ "api" }
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
