using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Security.JWT.Events;
using Intent.Modules.Security.JWT.Templates.ConfigurationJWTAuthentication;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Security.JWT.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConfigurationJWTAuthenticationDefaultDecorator : JWTAuthenticationDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.JWT.ConfigurationJWTAuthenticationDefaultDecorator";

        private readonly ConfigurationJWTAuthenticationTemplate _template;
        private readonly IApplication _application;
        private bool _overrideBearerTokenConfiguration;

        [IntentManaged(Mode.Merge)]
        public ConfigurationJWTAuthenticationDefaultDecorator(ConfigurationJWTAuthenticationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            application.EventDispatcher.Subscribe<OverrideBearerTokenConfigurationEvent>(evt =>
            {
                _overrideBearerTokenConfiguration = true;
            });
        }

        public IEnumerable<string> DeclareUsings()
        {
            if (_overrideBearerTokenConfiguration) { return Enumerable.Empty<string>(); }

            return new[]
            {
                "Microsoft.AspNetCore.Authentication.JwtBearer",
                "System.IdentityModel.Tokens.Jwt"
            };
        }

        public override string GetConfigurationCode()
        {
            if (_overrideBearerTokenConfiguration) { return string.Empty; }

            return @"
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            // JWT tokens (default scheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = Configuration.GetSection(""Security.Bearer:Authority"").Get<string>();
                options.Audience = Configuration.GetSection(""Security.Bearer:Audience"").Get<string>();

                options.TokenValidationParameters.RoleClaimType = ""role"";
                options.SaveToken = true;
            })
            ;";
        }
    }
}