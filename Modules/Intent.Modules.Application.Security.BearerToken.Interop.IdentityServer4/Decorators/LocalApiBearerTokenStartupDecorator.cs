using Intent.Engine;
using Intent.Modules.Application.Security.BearerToken.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using System.Collections.Generic;

namespace Intent.Modules.Application.Security.BearerToken.Interop.IdentityServer4.Decorators
{
    public class LocalApiBearerTokenStartupDecorator : StartupDecorator, IDeclareUsings, IDecoratorExecutionHooks
    {
        public const string DecoratorId = "Application.Security.BearerToken.Interop.IdentityServer4.LocalApiBearerTokenStartupDecorator";

        public LocalApiBearerTokenStartupDecorator(IApplication application)
        {
            Priority = -9;
            Application = application;
        }

        public IApplication Application { get; }

        public void BeforeTemplateExecution()
        {
            Application.EventDispatcher.Publish(new OverrideBearerTokenConfigurationEvent());
        }

        public override string ConfigureServices()
        {
            return @"ConfigureAuthentication(services);";
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Microsoft.AspNetCore.Authentication.JwtBearer",
                "System.IdentityModel.Tokens.Jwt"
            };
        }

        public override string Methods()
        {
            return @"
private void ConfigureAuthentication(IServiceCollection services)
{
    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
    
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddLocalApi(JwtBearerDefaults.AuthenticationScheme, opt => 
        {
            opt.SaveToken = true;
        });
}";
        }
    }
}
