using Intent.Engine;
using Intent.Modules.Application.Security.BearerToken.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Application.Security.BearerToken.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class BearerTokenStartupDecorator : StartupDecorator, IDeclareUsings, IHasNugetDependencies
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.Security.BearerToken.BearerTokenStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;
        private bool _overrideBearerTokenConfiguration;

        public BearerTokenStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -9;
            application.EventDispatcher.Subscribe<OverrideBearerTokenConfigurationEvent>(evt =>
            {
                _overrideBearerTokenConfiguration = true;
            });
        }

        public override string Configuration()
        {
            if (_overrideBearerTokenConfiguration) { return string.Empty; }

            return "app.UseAuthentication();";
        }

        public override string ConfigureServices()
        {
            if (_overrideBearerTokenConfiguration) { return string.Empty; }

            return @"ConfigureAuthentication(services);";
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

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer
            };
        }

        public override string Methods()
        {
            if (_overrideBearerTokenConfiguration) { return string.Empty; }

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
            options.SaveToken = true;
        })
        ;
}";
        }
    }
}
