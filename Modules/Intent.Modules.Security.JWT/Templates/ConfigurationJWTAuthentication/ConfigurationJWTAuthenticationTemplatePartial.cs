using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Security.JWT.Templates.ConfigurationJWTAuthentication
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ConfigurationJWTAuthenticationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Security.JWT.ConfigurationJWTAuthentication";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ConfigurationJWTAuthenticationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer);
            CSharpFile = new CSharpFile($"{this.GetNamespace()}", $"{this.GetFolderPath()}")
                .AddUsing("System")
                .AddUsing("System.IdentityModel.Tokens.Jwt")
                .AddUsing("Microsoft.AspNetCore.Authentication.JwtBearer")
                .AddUsing("Microsoft.AspNetCore.Authorization")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.IdentityModel.Tokens")
                .AddClass("JWTAuthenticationConfiguration", @class => @class
                    .Static()
                    .AddMethod("IServiceCollection", "ConfigureJWTSecurity", method => method
                        .Static()
                        .WithComments("// Use '[IntentManaged(Mode.Ignore)]' on this method should you want to deviate from the standard JWT token approach")
                        .AddParameter("IServiceCollection", "services", parameter => parameter.WithThisModifier())
                        .AddParameter("IConfiguration", "configuration")
                        .AddStatement("JwtSecurityTokenHandler.DefaultMapInboundClaims = false;")
                        .AddStatement($"services.AddScoped<{this.GetCurrentUserServiceInterfaceName()}, {this.GetCurrentUserServiceName()}>();")
                        .AddStatement("services.AddHttpContextAccessor();")
                        .AddStatement(new CSharpMethodChainStatement("services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)")
                            .AddChainStatement(@"AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = configuration.GetSection(""Security.Bearer:Authority"").Get<string>();
                    options.Audience = configuration.GetSection(""Security.Bearer:Audience"").Get<string>();

                    options.TokenValidationParameters.RoleClaimType = ""role"";
                    options.SaveToken = true;
                })"))
                        .AddStatement("services.AddAuthorization(ConfigureAuthorization);")
                        .AddStatement("return services;", s => s.SeparatedFromPrevious()))
                    .AddMethod("void", "ConfigureAuthorization", method => method
                        .Static()
                        .AddAttribute("[IntentManaged(Mode.Ignore)]")
                        .AddParameter("AuthorizationOptions", "options")
                        .AddStatement("//Configure policies and other authorization options here. For example:")
                        .AddStatement("//options.AddPolicy(\"EmployeeOnly\", policy => policy.RequireClaim(\"role\", \"employee\"));")
                        .AddStatement("//options.AddPolicy(\"AdminOnly\", policy => policy.RequireClaim(\"role\", \"admin\"));")));
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}