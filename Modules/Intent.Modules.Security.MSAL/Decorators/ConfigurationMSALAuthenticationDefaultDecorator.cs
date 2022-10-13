using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Security.MSAL.Templates.ConfigurationMSALAuthentication;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConfigurationMSALAuthenticationDefaultDecorator : MSALAuthenticationDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.MSAL.ConfigurationMSALAuthenticationDefaultDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly ConfigurationMSALAuthenticationTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public ConfigurationMSALAuthenticationDefaultDecorator(ConfigurationMSALAuthenticationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Microsoft.AspNetCore.Authentication",
                "Microsoft.AspNetCore.Authentication.JwtBearer",
                "Microsoft.AspNetCore.Authentication.OpenIdConnect",
                "Microsoft.AspNetCore.Authorization",
                "Microsoft.Identity.Web",
                "System.IdentityModel.Tokens.Jwt"
            };
        }

        public override string GetConfigurationCode()
        {
            return @"
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration.GetSection(""AzureAd""));
        
        services.Configure<OpenIdConnectOptions>(
            OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.RoleClaimType = ""roles"";
                options.TokenValidationParameters.NameClaimType = ""name"";
            });";
        }
    }
}