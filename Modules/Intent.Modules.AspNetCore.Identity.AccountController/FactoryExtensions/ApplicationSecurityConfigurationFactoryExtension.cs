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

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("JwtConfiguration"));

            var @class = template?.CSharpFile.Classes.SingleOrDefault(x => x.Name == "JWTAuthenticationConfiguration");
            var method = @class?.Methods.SingleOrDefault(x => x.Name == "ConfigureJWTSecurity");
            var statement = method?.Statements.SingleOrDefault(x => x.ToString().Contains("services.AddAuthentication(")) as CSharpMethodChainStatement;
            var chainStatement = statement?.Statements.SingleOrDefault(x => x.ToString().StartsWith("AddJwtBearer("));

            chainStatement?.Replace(@"AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration.GetSection(""JwtToken:Issuer"").Get<string>(),
                        ValidateAudience = true,
                        ValidAudience = configuration.GetSection(""JwtToken:Audience"").Get<string>(),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration.GetSection(""JwtToken:SigningKey"").Get<string>()!))
                    };
                    options.SaveToken = true;
                })");
        }
    }
}