using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
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
            });
        }
    }
}
