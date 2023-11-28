using System.ComponentModel.DataAnnotations;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Identity.Templates.AspNetCoreIdentityConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

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
            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            startupTemplate.CSharpFile.OnBuild(file =>
            {
                var startup = startupTemplate.StartupFile;

                // ConfigureServices method:
                {
                    file.AddUsing("Microsoft.Extensions.DependencyInjection");
                    startup.ConfigureServices((block, ctx) =>
                    {
                        if (block.Statements.All(x => !x.ToString()!.Contains(".AddRazorPages()")))
                        {
                            var insertAfter = block.Statements
                                .Single(x => x.ToString()!.Contains(".AddInfrastructure("));
                            insertAfter.InsertBelow(new CSharpInvocationStatement($"{ctx.Services}.AddRazorPages"));
                        }
                    });
                }

                // Configure method:
                {
                    // app.UseStaticFiles();
                    startup.ConfigureApp((block, ctx) =>
                    {
                        if (block.Statements.All(x => !x.ToString()!.Contains(".UseStaticFiles")))
                        {
                            var insertAfter = block.Statements
                                .Single(x => x.ToString()!.Contains(".UseHttpsRedirection("));
                            insertAfter.InsertBelow(new CSharpInvocationStatement($"{ctx.App}.UseStaticFiles"));
                        }
                    });

                    // endpoints.MapRazorPages();
                    startup.ConfigureEndpoints((block, ctx) =>
                    {
                        if (block.Statements.All(x => !x.ToString()!.Contains(".MapRazorPages")))
                        {
                            var insertAfter = block.Statements
                                .Single(x => x.ToString()!.Contains(".MapControllers("));
                            insertAfter.InsertBelow(new CSharpInvocationStatement($"{ctx.Endpoints}.MapRazorPages"));
                        }
                    });
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