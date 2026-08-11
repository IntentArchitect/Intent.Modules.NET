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
using Intent.Modules.Common.Templates;
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
            // AppStartup is host-scoped and shared by every ASP.NET Core host, including plain API
            // hosts with no Blazor components. MudBlazor must be wired only into the host(s) that
            // actually have Blazor components, so anchor on AppRazorTemplate (unique to Blazor hosts)
            // and scope the startup lookup to its own output target, rather than the application-wide
            // lookup, which throws once a second host exists.
            var appRazorTemplates = application.FindTemplateInstances<IIntentTemplate>(AppRazorTemplate.TemplateId).ToArray();

            if (appRazorTemplates.Length == 0)
            {
                Logging.Log.Warning("Unable to install Blazorise. Startup class could not be found.");
                return;
            }

            foreach (var appRazorTemplate in appRazorTemplates)
            {
                var startup = appRazorTemplate.OutputTarget.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
                if (startup == null)
                {
                    continue;
                }

                startup.AddNugetDependency(NugetPackages.MudBlazor(startup.OutputTarget));

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
        }

        private void RegisterInClientProgram(IApplication application)
        {
            // Blazor client Program is host-scoped, and a multi-host application can have a Blazor
            // WebAssembly client in more than one host - loop every instance instead of the singular,
            // application-wide lookup, which throws once a second host exists.
            foreach (var program in application.FindTemplateInstances<IBlazorProgramTemplate>(ProgramTemplate.TemplateId))
            {
                program.AddNugetDependency(NugetPackages.MudBlazor(program.OutputTarget));

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
        }

        private void UpdateClientGlobalImports(IApplication application)
        {
            // Host-scoped - see RegisterInClientProgram for why this loops instead of a singular lookup.
            foreach (var imports in application.FindTemplateInstances<IRazorFileTemplate>(ClientImportsRazorTemplate.TemplateId))
            {
                imports.RazorFile.AddUsing("MudBlazor");
                imports.RazorFile.AddUsing("MudBlazor.Services");
            }
        }

        private void UpdateServerGlobalImports(IApplication application)
        {
            // Host-scoped - see RegisterInClientProgram for why this loops instead of a singular lookup.
            foreach (var imports in application.FindTemplateInstances<IRazorFileTemplate>(ServerImportsRazorTemplate.TemplateId))
            {
                imports.RazorFile.AddUsing("MudBlazor");
                imports.RazorFile.AddUsing("MudBlazor.Services");
            }
        }

        private void UpdateAppRazorFile(IApplication application)
        {
            // Host-scoped - see RegisterInClientProgram for why this loops instead of a singular lookup.
            var appRazorTemplates = application.FindTemplateInstances<IRazorFileTemplate>(AppRazorTemplate.TemplateId).ToArray();

            if (appRazorTemplates.Length == 0)
            {
                Logging.Log.Warning("Unable to install Blazorise. Program class could not be found.");
                return;
            }

            foreach (var appRazorTemplate in appRazorTemplates)
            {
                var app = appRazorTemplate.RazorFile;
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

                    foreach (var link in file.SelectHtmlElements("/html/head/link"))
                    {
                        // Remove Bootstrap:
                        if (link.HasAttribute("rel", "stylesheet") && link.GetAttribute("href")?.Value.StartsWith("bootstrap") == true)
                        {
                            link.Remove();
                            continue;
                        }
                    }
                });
            }
        }

        private void UpdateRoutesRazorFile(IApplication application)
        {
            // Host-scoped - see RegisterInClientProgram for why this loops instead of a singular lookup.
            var routesTemplates = application.FindTemplateInstances<IRazorFileTemplate>(RoutesRazorTemplate.TemplateId).ToArray();

            if (routesTemplates.Length == 0)
            {
                Logging.Log.Warning("Unable to install MudBlazor. Program class could not be found.");
                return;
            }

            foreach (var routesTemplate in routesTemplates)
            {
                routesTemplate.RazorFile.OnBuild(file =>
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
}