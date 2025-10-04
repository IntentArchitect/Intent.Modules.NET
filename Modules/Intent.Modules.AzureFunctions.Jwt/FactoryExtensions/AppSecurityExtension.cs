using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Application.Identity.Templates.ApplicationSecurityConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Jwt.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AppSecurityExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AzureFunctions.Jwt.AppSecurityExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ApplicationSecurityConfigurationTemplate>(
                TemplateDependency.OnTemplate(ApplicationSecurityConfigurationTemplate.TemplateId));
            
            if (template == null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("System.IdentityModel.Tokens.Jwt");
                
                var priClass = file.Classes.First();
                var configMethod = priClass.FindMethod("ConfigureApplicationSecurity");
                
                if (configMethod == null)
                {
                    return;
                }
                
                var returnStmt = configMethod.FindStatement(stmt => stmt.GetText("").Contains("return services"));

                returnStmt.InsertAbove([
                    new CSharpStatement("services.AddHttpContextAccessor();"),
                    new CSharpStatement("services.AddSingleton<JwtSecurityTokenHandler>();"),
                ]);
            }, 1);
        }
    }
}