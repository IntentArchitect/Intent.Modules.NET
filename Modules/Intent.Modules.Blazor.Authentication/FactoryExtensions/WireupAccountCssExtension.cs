using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
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
    public class WireupAccountCssExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Authentication.WireupAccountCssExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // ux-account.css only ships for the non-MudBlazor, ASP.NET Core Identity account pages.
            if (!application.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
            {
                return;
            }

            if (application.InstalledModules.Any(im => im.ModuleId == "Intent.Blazor.Components.MudBlazor"))
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


    [IntentIgnore]
    public static class RazorFileExtensions
    {
        public static IEnumerable<IHtmlElement> SelectHtmlElements(this IRazorFile razorFile, string selector)
        {
            return razorFile.ChildNodes.OfType<IHtmlElement>().SelectHtmlElements(selector.Split("/", StringSplitOptions.RemoveEmptyEntries));
        }

        public static IEnumerable<IHtmlElement> SelectHtmlElements(this IEnumerable<IHtmlElement> nodes, string[] parts)
        {
            var firstPart = parts.FirstOrDefault();
            var foundNodes = nodes.Where(x => x.Name == firstPart).ToList();
            foreach (var found in foundNodes)
            {
                if (parts.Length == 1)
                {
                    yield return found;
                }

                foreach (var foundChildren in found.ChildNodes.OfType<IHtmlElement>().SelectHtmlElements(parts.Skip(1).ToArray()))
                {
                    yield return foundChildren;
                }
            }
        }
    }
}
