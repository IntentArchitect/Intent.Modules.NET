using System;
using System.Linq;
using System.Net;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
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
            var startup = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.Startup");
            if (startup is null)
            {
                return;
            }

            startup.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();

                var configureServicesBlock = @class.FindMethod("ConfigureServices");
                if (configureServicesBlock == null)
                {
                    return;
                }

                configureServicesBlock.AddStatement("services.AddRazorPages();");
                configureServicesBlock.AddStatement("services.AddServerSideBlazor();");

                var configureBlock = @class.FindMethod("Configure");
                if (configureBlock == null)
                {
                    return;
                }

                configureBlock.AddStatement("app.UseStaticFiles();");

                var ifDevelopmentStmt = (CSharpIfStatement)configureBlock.FindStatement(s => s is CSharpIfStatement cif && cif.GetText("").Contains("env.IsDevelopment()"));

                ifDevelopmentStmt.InsertBelow(new CSharpElseStatement(), s =>
                {
                    var elseStmt = (CSharpElseStatement)s;
                    elseStmt.AddStatement("app.UseExceptionHandler(\"/Error\");");
                    elseStmt.AddStatement("// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.");
                    elseStmt.AddStatement("app.UseHsts();");
                });

                var useEndpointsBlock = (IHasCSharpStatements)configureBlock.FindStatement(s => s.GetText("").Contains("UseEndpoints"));
                if (useEndpointsBlock == null)
                {
                    return;
                }

                useEndpointsBlock.AddStatement("endpoints.MapBlazorHub();");
                useEndpointsBlock.AddStatement("endpoints.MapFallbackToPage(\"/_Host\");");

            });
        }
    }
}