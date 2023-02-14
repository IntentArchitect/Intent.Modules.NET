using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Identity.Templates.AspNetCoreIdentityConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.UI.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AspNetCoreIdentityUiFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Identity.UI.AspNetCoreIdentityUiFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 10;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var startupTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("App.Startup");
            startupTemplate.CSharpFile.AfterBuild(file =>
            {
                // ConfigureServices method:
                {
                    file.AddUsing("Microsoft.Extensions.DependencyInjection");

                    var method = file.Classes
                        .SelectMany(x => x.Methods)
                        .Single(x => x.Name == "ConfigureServices");
                    if (method.Statements.All(x => !x.ToString().Contains("services.AddRazorPages()")))
                    {
                        var insertAfter = method.Statements
                            .Single(x => x.ToString().StartsWith("services.AddInfrastructure("));
                        insertAfter.InsertBelow(new CSharpInvocationStatement("services.AddRazorPages"));
                    }
                }

                // Configure method:
                {
                    var method = file.Classes
                        .SelectMany(x => x.Methods)
                        .Single(x => x.Name == "Configure");

                    // app.UseStaticFiles();
                    if (method.Statements.All(x => !x.ToString().Contains("app.UseStaticFiles")))
                    {
                        var insertAfter = method.Statements
                            .Single(x => x.ToString().StartsWith("app.UseHttpsRedirection("));
                        insertAfter.InsertBelow(new CSharpInvocationStatement("app.UseStaticFiles"));
                    }

                    // endpoints.MapRazorPages();
                    var endpointsStatement = method.Statements
                        .OfType<IHasCSharpStatements>()
                        .Single(x => x.ToString()!.Contains("app.UseEndpoints"));
                    if (endpointsStatement.Statements.All(x => !x.ToString().Contains("endpoints.MapRazorPages()")))
                    {
                        var insertAfter = endpointsStatement.Statements
                            .Single(x => x.ToString().StartsWith("endpoints.MapControllers("));
                        insertAfter.InsertBelow(new CSharpInvocationStatement("endpoints.MapRazorPages"));
                    }
                }
            });

            var identityConfigurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(AspNetCoreIdentityConfigurationTemplate.TemplateId);
            identityConfigurationTemplate.CSharpFile.AfterBuild(file =>
            {
                var statements = file.Classes
                    .SelectMany(x => x.Methods)
                    .SelectMany(x => x.Statements);

                foreach (var statement in statements)
                {
                    statement.FindAndReplace("services.AddIdentity<IdentityUser, IdentityRole>()", "services.AddDefaultIdentity<IdentityUser>()");
                }
            });
        }
    }
}