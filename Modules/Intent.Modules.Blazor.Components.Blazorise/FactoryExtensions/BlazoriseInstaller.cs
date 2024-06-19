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

namespace Intent.Modules.Blazor.Components.Blazorise.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BlazoriseInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Components.Blazorise.BlazoriseInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

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

            startup.AddNugetDependency(new NugetPackageInfo("Blazorise", "1.5.1"));
            startup.AddNugetDependency(new NugetPackageInfo("Blazorise.Icons.FontAwesome", "1.5.1"));
            startup.AddNugetDependency(new NugetPackageInfo("Blazorise.Tailwind", "1.5.1"));

            startup.AddUsing("Blazorise");
            startup.AddUsing("Blazorise.Icons.FontAwesome");
            startup.AddUsing("Blazorise.Tailwind");

            startup.CSharpFile.AfterBuild(file =>
            {
                startup.StartupFile.ConfigureServices((statements, context) =>
                {
                    var addBlazoriseServices = new CSharpMethodChainStatement($"{context.Services}.AddBlazorise()");
                    addBlazoriseServices.AddChainStatement("AddTailwindProviders()")
                        .AddChainStatement("AddFontAwesomeIcons()")
                        .WithSemicolon();
                    statements.AddStatement(addBlazoriseServices);
                });
            });
        }

        private void RegisterInClientProgram(IApplication application)
        {
            var program = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(ProgramTemplate.TemplateId);

            if (program == null)
            {
                Logging.Log.Warning("Unable to install Blazorise. Program class could not be found.");
                return;
            }

            program.AddNugetDependency(new NugetPackageInfo("Blazorise", "1.5.1"));
            program.AddNugetDependency(new NugetPackageInfo("Blazorise.Icons.FontAwesome", "1.5.1"));
            program.AddNugetDependency(new NugetPackageInfo("Blazorise.Tailwind", "1.5.1"));

            program.AddUsing("Blazorise");
            program.AddUsing("Blazorise.Icons.FontAwesome");
            program.AddUsing("Blazorise.Tailwind");

            program.CSharpFile.AfterBuild(file =>
            {
                file.Classes.First().FindMethod("Main").FindStatement(x => x.HasMetadata("run-builder"))
                    ?.InsertAbove(new CSharpMethodChainStatement("builder.Services.AddBlazorise()")
                        .AddChainStatement("AddTailwindProviders()")
                        .AddChainStatement("AddFontAwesomeIcons()")
                        .SeparatedFromNext());
            });
        }

        private void UpdateClientGlobalImports(IApplication application)
        {
            var imports = application.FindTemplateInstance<IRazorFileTemplate>(ClientImportsRazorTemplate.TemplateId);
            imports?.RazorFile.AddUsing("Blazorise");
        }

        private void UpdateServerGlobalImports(IApplication application)
        {
            var imports = application.FindTemplateInstance<IRazorFileTemplate>(ServerImportsRazorTemplate.TemplateId);
            imports?.RazorFile.AddUsing("Blazorise");
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
                var baseElement = file.SelectHtmlElement("/head/base");
                if (baseElement != null)
                {
                    baseElement.AddBelow(
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&amp;display=swap"),
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "https://unpkg.com/flowbite@1.5.4/dist/flowbite.min.css"),
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "_content/Blazorise.Icons.FontAwesome/v6/css/all.min.css"),
                        new EmptyLine(app),
                        new HtmlElement("script", app)
                            .AddAttribute("src", "https://cdn.tailwindcss.com"),
                        new HtmlElement("script", app)
                            .AddAttribute("src", "_content/Blazorise.Tailwind/blazorise.tailwind.config.js?v=1.5.0.0"),
                        new EmptyLine(app),
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "_content/Blazorise/blazorise.css?v=1.5.0.0"),
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "_content/Blazorise.Tailwind/blazorise.tailwind.css?v=1.5.0.0"),
                        new EmptyLine(app));
                }

                var routes = file.SelectHtmlElement("/body/Routes");
                routes.AddBelow(new HtmlElement("script", app)
                    .AddAttribute("src", "https://unpkg.com/flowbite@1.5.4/dist/flowbite.js"));


                // Remove Bootstrap:
                foreach (var link in file.SelectHtmlElements("/head/link"))
                {
                    if (link.HasAttribute("rel", "stylesheet") && link.GetAttribute("href")?.Value.StartsWith("bootstrap") == true)
                    {
                        link.Remove();
                    }
                }
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
                var routerNode = file.ChildNodes.SingleOrDefault(x => x is HtmlElement html && html.Name == "Router");
                file.ChildNodes.Remove(routerNode);
                file.AddHtmlElement("Blazorise.ThemeProvider", themeProvider =>
                {
                    themeProvider.AddAttribute("Theme", "@theme");
                    themeProvider.AddChildNode(routerNode);
                    themeProvider.AddHtmlElement("MessageProvider");
                    themeProvider.AddHtmlElement("PageProgressProvider");
                });


                file.AddCodeBlock(block =>
                {
                    block.AddField("Theme", "theme", prop =>
                    {
                        prop.Private().WithAssignment(new CSharpStatement(@"new()
    {
        BarOptions = new()
        {
            HorizontalHeight = ""72px""
        },
        ColorOptions = new()
        {
            Primary = ""#0288D1"",
            Secondary = ""#A65529"",
            Success = ""#23C02E"",
            Info = ""#9BD8FE"",
            Warning = ""#F8B86C"",
            Danger = ""#F95741"",
            Light = ""#F0F0F0"",
            Dark = ""#535353"",
        },
        BackgroundOptions = new()
        {
            Primary = ""#0288D1"",
            Secondary = ""#A65529"",
            Success = ""#23C02E"",
            Info = ""#9BD8FE"",
            Warning = ""#F8B86C"",
            Danger = ""#F95741"",
            Light = ""#F0F0F0"",
            Dark = ""#535353"",
        },
        InputOptions = new()
        {
            CheckColor = ""#0288D1"",
        }
    }"));
                    });
                });
            });
        }
    }
}