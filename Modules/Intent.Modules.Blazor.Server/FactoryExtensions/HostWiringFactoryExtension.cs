using System;
using System.Linq;
using System.Net;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Server.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HostWiringFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Server.HostWiringFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            RegisterStartup(application);
        }

        private void RegisterStartup(IApplication application)
        {
            var startup = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            startup?.CSharpFile.AfterBuild(file =>
            {
                startup.StartupFile.ConfigureServices((statements, context) =>
                {
                    statements.AddStatement($"{context.Services}.AddRazorPages();");
                    statements.AddStatement($"{context.Services}.AddServerSideBlazor();");
                });

                startup.StartupFile.ConfigureApp((statements, context) =>
                {
                    var ifDevelopmentStatement = (CSharpIfStatement)statements
                        .FindStatement(m => m is CSharpIfStatement cif && cif.GetText("").Contains("env.IsDevelopment()"));

                    if (ifDevelopmentStatement is not null)
                    {
                        ifDevelopmentStatement.InsertBelow(new CSharpElseStatement(), statement =>
                        {
                            var elseStatement = (CSharpElseStatement)statement;

                            elseStatement.AddStatement("app.UseExceptionHandler(\"/Error\");");
                            elseStatement.AddStatement("// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.");
                            elseStatement.AddStatement("app.UseHsts();");
                        });
                    }

                    statements.AddStatement("app.UseStaticFiles();");
                });

                startup.StartupFile.ConfigureEndpoints((statements, context) =>
                {
                    statements.AddStatement($"{context.Endpoints}.MapBlazorHub();");
                    statements.AddStatement($"{context.Endpoints}.MapFallbackToPage(\"/_Host\");");
                });
            });
        }
    }
}