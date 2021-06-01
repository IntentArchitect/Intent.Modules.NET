using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Security.JWT.Events;
using Intent.Modules.Security.JWT.Templates.ConfigurationJWTAuthentication;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Security.BearerToken.Interop.IdentityServer4.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConfigurationJWTAuthenticationLocalApiBearerTokenDecorator : JWTAuthenticationDecorator, IDeclareUsings, IDecoratorExecutionHooks
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.BearerToken.Interop.IdentityServer4.ConfigurationJWTAuthenticationLocalApiBearerTokenDecorator";

        private readonly ConfigurationJWTAuthenticationTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public ConfigurationJWTAuthenticationLocalApiBearerTokenDecorator(ConfigurationJWTAuthenticationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -10;
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new OverrideBearerTokenConfigurationEvent());
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Microsoft.AspNetCore.Authentication.JwtBearer",
                "System.IdentityModel.Tokens.Jwt"
            };
        }

        public override string GetConfigurationCode()
        {
            return @"
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddLocalApi(JwtBearerDefaults.AuthenticationScheme, opt => 
            {
                opt.SaveToken = true;
            });";
        }
    }
}
