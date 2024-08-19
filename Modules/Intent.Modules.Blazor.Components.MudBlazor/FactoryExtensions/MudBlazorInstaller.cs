using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.ClientImportsRazor;
using Intent.Modules.Blazor.Templates.Templates.Client.Program;
using Intent.Modules.Blazor.Templates.Templates.Client.RoutesRazor;
using Intent.Modules.Blazor.Templates.Templates.Server.AppRazor;
using Intent.Modules.Blazor.Templates.Templates.Server.ServerImportsRazor;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MudBlazorInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Components.MudBlazor.MudBlazorInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 100;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            RegisterInServerStartup(application);
            UpdateServerGlobalImports(application);

            RegisterInClientProgram(application);
            UpdateClientGlobalImports(application);
            UpdateRoutesRazorFile(application);
            UpdateAppRazorFile(application);
        }

        private void RegisterInServerStartup(IApplication application)
        {
            var startup = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            if (startup == null)
            {
                Logging.Log.Warning("Unable to install Blazorise. Startup class could not be found.");
                return;
            }

            startup.AddNugetDependency(new NugetPackageInfo("MudBlazor", "6.19.1"));

            startup.AddUsing("MudBlazor.Services");

            startup.CSharpFile.AfterBuild(file =>
            {
                startup.StartupFile.ConfigureServices((statements, context) =>
                {
                    var addMudServices = new CSharpMethodChainStatement($"{context.Services}.AddMudServices()");
                    statements.AddStatement(addMudServices);
                });
            });
        }

        private void RegisterInClientProgram(IApplication application)
        {
            var program = application.FindTemplateInstance<IBlazorProgramTemplate>(ProgramTemplate.TemplateId);

            if (program == null)
            {
                Logging.Log.Warning("Unable to install Blazorise. Program class could not be found.");
                return;
            }

            program.AddNugetDependency(new NugetPackageInfo("MudBlazor", "6.19.1"));

            program.AddUsing("MudBlazor.Services");

            program.CSharpFile.AfterBuild(_ =>
            {
                program.ProgramFile.ConfigureMainStatementsBlock(main =>
                {
                    main.FindStatement(x => x.HasMetadata("run-builder"))
                        ?.InsertAbove(new CSharpMethodChainStatement("builder.Services.AddMudServices()").SeparatedFromNext());
                });
            });
        }

        private void UpdateClientGlobalImports(IApplication application)
        {
            var imports = application.FindTemplateInstance<IRazorFileTemplate>(ClientImportsRazorTemplate.TemplateId);
            imports?.RazorFile.AddUsing("MudBlazor");
            imports?.RazorFile.AddUsing("MudBlazor.Services");
        }

        private void UpdateServerGlobalImports(IApplication application)
        {
            var imports = application.FindTemplateInstance<IRazorFileTemplate>(ServerImportsRazorTemplate.TemplateId);
            imports?.RazorFile.AddUsing("MudBlazor");
            imports?.RazorFile.AddUsing("MudBlazor.Services");
        }

        private void UpdateAppRazorFile(IApplication application)
        {
            var app = application.FindTemplateInstance<IRazorFileTemplate>(AppRazorTemplate.TemplateId)?.RazorFile;

            if (app == null)
            {
                Logging.Log.Warning("Unable to install Blazorise. Program class could not be found.");
                return;
            }

            app.OnBuild(file =>
            {
                // Add Blazorise dependencies
                var baseElement = file.SelectHtmlElements("/html/head/link").SingleOrDefault(x => x.HasAttribute("href", "app.css"));
                if (baseElement != null)
                {
                    baseElement.AddAbove(
                        new EmptyLine(app),
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap"),
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "_content/MudBlazor/MudBlazor.min.css"),
                        new EmptyLine(app));
                }

                var routes = file.SelectHtmlElements("/html/body/script").Last();
                routes.AddBelow(new HtmlElement("script", app)
                    .AddAttribute("src", "_content/MudBlazor/MudBlazor.min.js"));


                // Remove Bootstrap:
                //foreach (var link in file.SelectHtmlElements("/head/link"))
                //{
                //    if (link.HasAttribute("rel", "stylesheet") && link.GetAttribute("href")?.Value.StartsWith("bootstrap") == true)
                //    {
                //        link.Remove();
                //    }
                //}
            });
        }

        private void UpdateRoutesRazorFile(IApplication application)
        {
            var routes = application.FindTemplateInstance<IRazorFileTemplate>(RoutesRazorTemplate.TemplateId)?.RazorFile;

            if (routes == null)
            {
                Logging.Log.Warning("Unable to install Blazorise. Program class could not be found.");
                return;
            }

            routes.OnBuild(file =>
            {
                //var routerNode = file.ChildNodes.SingleOrDefault(x => x is HtmlElement html && html.Name == "Router");
                //file.ChildNodes.Remove(routerNode);
                //file.AddHtmlElement("Blazorise.ThemeProvider", themeProvider =>
                //{
                //    themeProvider.AddAttribute("Theme", "@theme");
                //    themeProvider.AddChildNode(routerNode);
                //    themeProvider.AddHtmlElement("MessageProvider");
                //    themeProvider.AddHtmlElement("PageProgressProvider");
                //});
            });
        }
    }
}