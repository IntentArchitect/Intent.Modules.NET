using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.PersistentAuthenticationStateProvider;
using Intent.Modules.Blazor.Templates.Templates.Client.Program;
using Intent.Modules.Blazor.Templates.Templates.Client.RoutesRazor;
using Intent.Modules.Blazor.Templates.Templates.Server.AppRazor;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using System.Threading;
using Intent.Modules.Blazor.Api;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientAddAuthentication : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Authentication.ClientAddAuthentication";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var program = application.FindTemplateInstance<IBlazorProgramTemplate>(ProgramTemplate.TemplateId);

            if (program == null)
            {
                Logging.Log.Warning("Unable to install authentication. Program class could not be found.");
                return;
            }

            program.AddUsing("Microsoft.AspNetCore.Components.Authorization");

            program.CSharpFile.AfterBuild(_ =>
            {
                if (program.ExecutionContext.GetSettings().GetGroup("489a67db-31b2-4d51-96d7-52637c3795be").GetSetting("3e3d24f8-ad29-44d6-b7e5-e76a5af2a7fa").Value != "interactive-server")
                {
                    program.ProgramFile.ConfigureMainStatementsBlock(main =>
                    {
                        main.FindStatement(x => x.HasMetadata("run-builder"))
                            ?.InsertAbove(new CSharpMethodChainStatement("builder.Services.AddCascadingAuthenticationState()").SeparatedFromNext())
                            ?.InsertAbove(new CSharpMethodChainStatement($"builder.Services.AddSingleton<AuthenticationStateProvider, {program.GetTypeName(PersistentAuthenticationStateProviderTemplate.TemplateId)}>()").SeparatedFromNext())
                            ?.InsertAbove(new CSharpMethodChainStatement($"builder.Services.AddApiAuthorization();").SeparatedFromNext());
                    });
                }
            });

            var routes = application.FindTemplateInstance<IRazorFileTemplate>(RoutesRazorTemplate.TemplateId);

            if (routes == null)
            {
                Logging.Log.Warning("Unable to install authentication in Routes.razor.");
                return;
            }

            var app = application.FindTemplateInstance<IRazorFileTemplate>(AppRazorTemplate.TemplateId)?.RazorFile;

            if (app == null)
            {
                Logging.Log.Warning("Unable to install auth-css. App.razor could not be found.");
                return;
            }

            app.OnBuild(file =>
            {
                // Add Blazorise dependencies
                var baseElement = file.SelectHtmlElements("/html/head/link").SingleOrDefault(x => x.HasAttribute("href", "app.css"));
                if (baseElement != null)
                {
                    baseElement.AddBelow(
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "auth-forms.css"));
                }
            });
        }
    }
}