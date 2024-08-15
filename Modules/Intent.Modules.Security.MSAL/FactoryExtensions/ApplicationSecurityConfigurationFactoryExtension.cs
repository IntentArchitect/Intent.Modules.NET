using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates.ApplicationSecurityConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ApplicationSecurityConfigurationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Security.MSAL.ApplicationSecurityConfigurationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ApplicationSecurityConfigurationTemplate>(TemplateDependency.OnTemplate(ApplicationSecurityConfigurationTemplate.TemplateId));
            if (template == null)
            {
                return;
            }

            template.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer(template.OutputTarget));
            template.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationOpenIdConnect(template.OutputTarget));
            template.AddNugetDependency(NugetPackages.MicrosoftIdentityWeb(template.OutputTarget));
            template.AddNugetDependency(NugetPackages.MicrosoftIdentityWebMicrosoftGraph(template.OutputTarget));
            template.AddNugetDependency(NugetPackages.MicrosoftIdentityWebUI(template.OutputTarget));

            template.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("System");
                file.AddUsing("System.Collections.Generic");
                file.AddUsing("Microsoft.Extensions.Configuration");
                file.AddUsing("Microsoft.Extensions.DependencyInjection");
                file.AddUsing("Microsoft.AspNetCore.Authorization");
                file.AddUsing("Microsoft.AspNetCore.Authentication.JwtBearer");
                file.AddUsing("Microsoft.AspNetCore.Authentication.OpenIdConnect");
                file.AddUsing("Microsoft.AspNetCore.Authorization");
                file.AddUsing("Microsoft.Identity.Web");
                file.AddUsing("System.IdentityModel.Tokens.Jwt");

                var priClass = file.Classes.First();
                var configMethod = priClass.FindMethod("ConfigureApplicationSecurity");
                configMethod.FindStatement(stmt => stmt.GetText("").Contains("return services")).Remove();
                configMethod.AddStatement("JwtSecurityTokenHandler.DefaultMapInboundClaims = false;");
                configMethod.AddStatement("services.AddHttpContextAccessor();");
                configMethod.AddMethodChainStatement("services", stmt => stmt
                    .AddChainStatement("AddAuthentication(JwtBearerDefaults.AuthenticationScheme)")
                    .AddChainStatement(@"AddMicrosoftIdentityWebApi(configuration.GetSection(""AzureAd""))"));
                configMethod.AddInvocationStatement("services.Configure<OpenIdConnectOptions>", block => block
                    .AddArgument("OpenIdConnectDefaults.AuthenticationScheme")
                    .AddArgument(new CSharpLambdaBlock("options")
                        .AddStatement(@"options.TokenValidationParameters.RoleClaimType = ""roles"";")
                        .AddStatement(@"options.TokenValidationParameters.NameClaimType = ""name"";"))
                    .WithArgumentsOnNewLines());
                configMethod.AddMethodChainStatement("services.AddAuthorization(ConfigureAuthorization)", stmt => stmt.AddMetadata("add-authorization", true));
                configMethod.AddStatement("return services;", s => s.SeparatedFromPrevious());

                priClass.AddMethod("void", "ConfigureAuthorization", method => method
                    .Private().Static()
                    .AddAttribute(CSharpIntentManagedAttribute.Ignore())
                    .AddParameter("AuthorizationOptions", "options")
                    .AddStatement("//Configure policies and other authorization options here. For example:")
                    .AddStatement("//options.AddPolicy(\"EmployeeOnly\", policy => policy.RequireClaim(\"role\", \"employee\"));")
                    .AddStatement("//options.AddPolicy(\"AdminOnly\", policy => policy.RequireClaim(\"role\", \"admin\"));"));
            });

            template.ApplyAppSetting("AzureAd", new
            {
                Instance = "https://login.microsoftonline.com/",
                Domain = "[Enter the domain of your tenant, e.g. contoso.onmicrosoft.com]",
                TenantId = "[Enter 'common', or 'organizations' or the Tenant Id (Obtained from the Azure portal. Select 'Endpoints' from the 'App registrations' blade and use the GUID in any of the URLs), e.g. da41245a5-11b3-996c-00a8-4d99re19f292]",
                ClientId = "[Enter the Client Id (Application ID obtained from the Azure portal), e.g. ba74781c2-53c2-442a-97c2-3d60re42f403]",
                Audience = "[Enter the ClientId]",
                SignUpSignInPolicyId = "[Policy Id(User Flow Name or Policy Id)]",
                Scopes = "[list of required scopes separated by space \"api://[Client Id here]/App.Read api://[Client Id here]/App.Write\"]",
                CallbackPath = "/signin-oidc",
                SignedOutCallbackPath = "/signout-callback-oidc",
                ClientSecret = "[Copy the client secret added to the app from the Azure portal]"
            });
        }
    }
}