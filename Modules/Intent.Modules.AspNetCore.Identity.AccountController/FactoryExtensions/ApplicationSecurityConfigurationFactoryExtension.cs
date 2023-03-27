using System.Linq;
using Intent.Engine;
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
                var authStatement = priClass.FindMethod("ConfigureApplicationSecurity")
                    .FindStatement(stmt => stmt.HasMetadata("add-authorization")) as CSharpMethodChainStatement;
                var chainStatement = authStatement?.Statements.SingleOrDefault(x => x.ToString().StartsWith("AddJwtBearer("));

                chainStatement?.Replace(new CSharpInvocationStatement("AddJwtBearer")
                    .AddArgument("JwtBearerDefaults.AuthenticationScheme")
                    .AddArgument(new CSharpLambdaBlock("options")
                        .AddStatement(new CSharpStatementBlock("options.TokenValidationParameters = new TokenValidationParameters")
                            .AddStatements($@"
            ValidateIssuer = true,
			ValidIssuer = configuration.GetSection(""JwtToken:Issuer"").Get<string>(),
			ValidateAudience = true,
			ValidAudience = configuration.GetSection(""JwtToken:Audience"").Get<string>(),
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration.GetSection(""JwtToken:SigningKey"").Get<string>()!))"))
                        .AddStatement("options.SaveToken = true;"))
                    .WithArgumentsOnNewLines());
            }, 5);
        }
    }
}