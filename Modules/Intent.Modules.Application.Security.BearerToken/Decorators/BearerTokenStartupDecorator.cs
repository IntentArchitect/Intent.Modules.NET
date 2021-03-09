using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.Security.BearerToken.Decorators
{
    public class BearerTokenStartupDecorator : StartupDecorator, IDeclareUsings, IHasNugetDependencies
    {
        public const string DecoratorId = "Application.Security.BearerTokenStartupDecorator";

        public BearerTokenStartupDecorator()
        {
            Priority = -9;
        }

        public override string Configuration()
        {
            return "app.UseAuthentication();";
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

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[] 
            {
                NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer
            };
        }

        public override string Methods()
        {
            return @"
private void ConfigureAuthentication(IServiceCollection services)
{
    // Use '[IntentManaged(Mode.Ignore)]' on this method should you want to deviate from the standard JWT token approach

    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        // JWT tokens (default scheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = Configuration.GetSection(""Security.Bearer:Authority"").Get<string>();
            options.Audience = Configuration.GetSection(""Security.Bearer:Audience"").Get<string>();

            options.TokenValidationParameters.RoleClaimType = ""role"";
        })
        ;
}";
        }
    }
}
