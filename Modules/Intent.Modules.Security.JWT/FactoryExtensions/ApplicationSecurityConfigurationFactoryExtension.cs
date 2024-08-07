using System.Linq;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates.ApplicationSecurityConfiguration;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Security.JWT.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class ApplicationSecurityConfigurationFactoryExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Security.JWT.ApplicationSecurityConfigurationFactoryExtension";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var template = application.FindTemplateInstance<ApplicationSecurityConfigurationTemplate>(TemplateDependency.OnTemplate(ApplicationSecurityConfigurationTemplate.TemplateId));
        if (template == null)
        {
            return;
        }

        template.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer(template.OutputTarget));
        template.AddNugetDependency(NugetPackages.IdentityModel(template.OutputTarget));
        template.CSharpFile.AfterBuild(file =>
        {
            file.AddUsing("System")
                .AddUsing("System.IdentityModel.Tokens.Jwt")
                .AddUsing("Microsoft.AspNetCore.Authentication.JwtBearer")
                .AddUsing("Microsoft.AspNetCore.Authorization")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.IdentityModel.Tokens");
            var priClass = file.Classes.First();

            var configMethod = priClass.FindMethod("ConfigureApplicationSecurity");
            configMethod.FindStatement(stmt => stmt.GetText("").Contains("return services")).Remove();
            configMethod.AddStatement("JwtSecurityTokenHandler.DefaultMapInboundClaims = false;")
                .AddStatement("services.AddHttpContextAccessor();")
                .AddStatement(new CSharpMethodChainStatement("services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)")
                    .AddChainStatement(new CSharpInvocationStatement("AddJwtBearer")
                        .AddArgument("JwtBearerDefaults.AuthenticationScheme")
                        .AddArgument(new CSharpLambdaBlock("options")
                            .AddStatements($@"
                    options.Authority = configuration.GetSection(""Security.Bearer:Authority"").Get<string>();
                    options.Audience = configuration.GetSection(""Security.Bearer:Audience"").Get<string>();

                    options.TokenValidationParameters.RoleClaimType = ""role"";
                    options.SaveToken = true;"))
                        .WithArgumentsOnNewLines())
                    .WithoutSemicolon()
                    .AddMetadata("add-authentication", true))
                .AddMethodChainStatement("services.AddAuthorization(ConfigureAuthorization)", stmt => stmt.AddMetadata("add-authorization", true))
                .AddStatement("return services;", s => s.SeparatedFromPrevious());

            priClass.AddMethod("void", "ConfigureAuthorization", method => method
                .Private().Static()
                .AddAttribute(CSharpIntentManagedAttribute.Ignore())
                .AddParameter("AuthorizationOptions", "options")
                .AddStatement("//Configure policies and other authorization options here. For example:")
                .AddStatement("//options.AddPolicy(\"EmployeeOnly\", policy => policy.RequireClaim(\"role\", \"employee\"));")
                .AddStatement("//options.AddPolicy(\"AdminOnly\", policy => policy.RequireClaim(\"role\", \"admin\"));"));
        }, 1);

    }
}