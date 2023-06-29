using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Identity.AccountController.Templates.TokenServiceConcrete;
using Intent.Modules.AspNetCore.Identity.AccountController.Templates.TokenServiceInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ApplicationSecurityConfigurationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Identity.AccountController.ApplicationSecurityConfigurationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Security.Configuration"));
            if (template == null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                var securityMethod = priClass.FindMethod("ConfigureApplicationSecurity");

                securityMethod.Statements.Insert(0, $@"services.AddTransient<{template.GetTypeName(TokenServiceInterfaceTemplate.TemplateId)}, {template.GetTypeName(TokenServiceConcreteTemplate.TemplateId)}>();");

                var authStatement = securityMethod.FindStatement(stmt => stmt.HasMetadata("add-authentication")) as CSharpMethodChainStatement;
                var chainStatement = authStatement?.Statements.SingleOrDefault(x => x.ToString().StartsWith("AddJwtBearer("));

                chainStatement?.Replace(new CSharpInvocationStatement("AddJwtBearer")
                    .AddArgument("JwtBearerDefaults.AuthenticationScheme")
                    .AddArgument(new CSharpLambdaBlock("options")
                        .AddStatement(new CSharpObjectInitializerBlock("options.TokenValidationParameters = new TokenValidationParameters")
                            .AddInitStatement("ValidateIssuer", "true")
                            .AddInitStatement("ValidIssuer", @"configuration.GetSection(""JwtToken:Issuer"").Get<string>()")
                            .AddInitStatement("ValidateAudience", "true")
                            .AddInitStatement("ValidAudience", @"configuration.GetSection(""JwtToken:Audience"").Get<string>()")
                            .AddInitStatement("ValidateIssuerSigningKey", "true")
                            .AddInitStatement("IssuerSigningKey", @"new SymmetricSecurityKey(Convert.FromBase64String(configuration.GetSection(""JwtToken:SigningKey"").Get<string>()!))")
                            .AddInitStatement("NameClaimType", @"""sub""")
                            .WithSemicolon())
                        .AddStatement(@"options.TokenValidationParameters.RoleClaimType = ""role"";")
                        .AddStatement("options.SaveToken = true;"))
                    .WithArgumentsOnNewLines());
            }, 2);
        }
    }
}