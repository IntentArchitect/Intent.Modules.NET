using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using static Intent.Blazor.Authentication.Api.SecurityConfigurationModelStereotypeExtensions.SecurityType;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WireupMudBlazorAccountCss : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Authentication.WireupMudBlazorAccountCss";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var securityType = application.MetadataManager.GetAuthenticationType(application.Id);

            // The MudBlazor variant of ux-account.css ships for MudBlazor apps with a local account
            // UI — Identity or JWT (!IsOidc()); OIDC redirects to an external IdP. The non-MudBlazor
            // case is handled by WireupAccountCssExtension.
            if (securityType.IsOIDC())
            {
                return;
            }

            if (!application.InstalledModules.Any(im => im.ModuleId == "Intent.Blazor.Components.MudBlazor"))
            {
                return;
            }

            var app = application.FindTemplateInstance<IRazorFileTemplate>("Intent.Blazor.Templates.Server.AppRazorTemplate")?.RazorFile;

            if (app == null)
            {
                Logging.Log.Warning("Unable to install ux-account.css. App.razor could not be found.");
                return;
            }

            app.AfterBuild(file =>
            {
                // Place ux-account.css after app.css — which is after ux-mudblazor.css — so the account
                // layer can override the shared MudBlazor page-header utility for account pages.
                var appCssLink = file.SelectHtmlElements("/html/head/link").SingleOrDefault(x => x.HasAttribute("href", "app.css"));
                if (appCssLink != null)
                {
                    appCssLink.AddBelow(
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "ux-account.css"));
                }
            }, 100);
        }
    }
}
