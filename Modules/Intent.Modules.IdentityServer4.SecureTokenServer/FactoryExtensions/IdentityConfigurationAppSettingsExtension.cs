using System;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.Common;
using Intent.Modules.Common.Configuration;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdentityConfigurationAppSettingsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.IdentityServer4.SecureTokenServer.IdentityConfigurationAppSettingsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        private string _sslPort = "client_port";

        protected override void OnAfterMetadataLoad(IApplication application)
        {
            application.EventDispatcher.Subscribe<HostingSettingsCreatedEvent>(x => Handle(x, application)); // This is just temporary. Need to store these settings in a solution-wide accessible space for each app.   
        }

        private void Handle(HostingSettingsCreatedEvent @event, IApplication application)
        {
            _sslPort = @event.SslPort.ToString();
            application.EventDispatcher.Publish(new SecureTokenServiceHostedEvent(_sslPort));
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var appName = application.Name;
            var template = application.FindTemplateInstance<IdentityServerConfigurationTemplate>(IdentityServerConfigurationTemplate.TemplateId);
            if (template is null)
            {
                return;
            }

            template.ApplyAppSetting("IdentityServer", new
            {
                Clients = new object[]
                {
                    new
                    {
                        Enabled = true,
                        ClientId = $"ResourceOwner_Client",
                        ClientName = $"{appName} Resource-Owner Client",
                        RequireClientSecret = false,
                        ClientSecrets = new []{ new { Value = "secret" } },
                        AllowedGrantTypes = new []{ "password" },
                        AllowedScopes = new []{ "api", "openid", "profile", "email" }
                    },
                    new
                    {
                        Enabled = true,
                        ClientId = $"ClientCredential_Client",
                        ClientName = $"{appName} Client-Credential Client",
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
                        ClientName = $"{appName} Authorize-Code Client",
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