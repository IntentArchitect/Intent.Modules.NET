using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AccountLayout;
using Intent.Modules.Blazor.Templates.Templates.Server.AppRazor;
using Intent.Modules.Blazor.Templates.Templates.Server.ServerImportsRazor;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeRenderMode : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Authentication.ChangeRenderMode";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var app = application.FindTemplateInstance<IRazorFileTemplate>(AppRazorTemplate.TemplateId)?.RazorFile;

            if (app == null)
            {
                Logging.Log.Warning("Unable to change rendermode. App.Razor file could not be found.");
                return;
            }

            app.OnBuild(file =>
            {
                // ASP.NET Core Identity account pages must run as static SSR so Identity can issue auth cookies.
                // Add theme attributes to <html> so those SSR pages render with the cookie-backed theme before interactivity.
                var htmlElement = file.ChildNodes.FirstOrDefault(n => n is IHtmlElement) as IHtmlElement;
                htmlElement?.AddAttribute("data-theme", "@_theme");
                htmlElement?.AddAttribute("data-theme-storage", BlazorThemeCapabilities.CookieThemeStorageValue);

                // The Mud manage shell (ManageLayout) uses a circuit-free overlay nav drawer toggled by
                // nav-drawer.js (mirrors theme-storage.js). Only ship the <script> for MudBlazor apps —
                // nav-drawer.js ships from the MudBlazor-gated ThemeMudBlazor wwwroot content.
                if (application.InstalledModules.Any(im => im.ModuleId == "Intent.Blazor.Components.MudBlazor"))
                {
                    var themeStorageScript = file.SelectHtmlElements("/html/body/script")
                        .FirstOrDefault(s => s.HasAttribute("src", "theme-storage.js"));
                    themeStorageScript?.AddBelow(new HtmlElement("script", app).AddAttribute("src", "nav-drawer.js"));
                }

                var razorCodeBlock = file.ChildNodes.FirstOrDefault(n => n is IRazorCodeBlock);

                if (razorCodeBlock is not null)
                {
                    var codeBlock = razorCodeBlock as ICSharpClass;

                    // The cookie-backed theme is needed because ASP.NET Core Identity forces account pages through static SSR.
                    codeBlock?.AddCodeBlock(
                        "private string _theme => HttpContext.Request.Cookies.TryGetValue(\"theme\", out var t) && t == \"light\" ? \"light\" : \"dark\";");

                    var renderModeMethod = codeBlock?.FindMethod("GetRenderModeForPage");

                    if (renderModeMethod is not null)
                    {
                        var cif = new CSharpIfStatement("HttpContext.Request.Path.StartsWithSegments(\"/Account\")");
                        cif.AddReturn(new CSharpStatement("null"));
                        renderModeMethod.InsertStatement(0, cif);
                    }
                }

                var imports = application.FindTemplateInstance<IRazorFileTemplate>(ServerImportsRazorTemplate.TemplateId);
                imports?.RazorFile.AddUsing("Microsoft.AspNetCore.Components.Authorization");

                // AppUserMenu (and the rest of Components/Account/Shared) is referenced from layouts OUTSIDE
                // the Account folder (e.g. the main Layout's <AppUserMenu/>). Razor applies a folder's own
                // _Imports only hierarchically, so contribute the Account/Shared namespace to the ROOT server
                // _Imports rather than an inline @using on the layout. (An inline using also gets stripped by
                // the razor weaver when the same namespace appears in any other _Imports.razor — see
                // .user/Module-Versioning.md notes on the weaver's project-wide _Imports flattening.)
                var accountShared = application.FindTemplateInstance<IRazorFileTemplate>(AccountLayoutTemplate.TemplateId);
                if (accountShared is not null)
                {
                    imports?.RazorFile.AddUsing(accountShared.Namespace);
                }
            });
        }
    }
}
