using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AI.Blazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WireupCssExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AI.Blazor.WireupCssExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var app = application.FindTemplateInstance<IRazorFileTemplate>("Intent.Blazor.Templates.Server.AppRazorTemplate")?.RazorFile;

            if (app == null)
            {
                Logging.Log.Warning("Unable to install ux-theme.css. App.razor could not be found.");
                return;
            }

            app.AfterBuild(file =>
            {
                // Add Blazorise dependencies
                var baseElement = file.SelectHtmlElements("/html/head/link").SingleOrDefault(x => x.HasAttribute("href", "_content/MudBlazor/MudBlazor.min.css"));
                if (baseElement != null)
                {
                    baseElement.AddBelow(
                        new HtmlElement("link", app)
                            .AddAttribute("rel", "stylesheet")
                            .AddAttribute("href", "ux-theme.css"));
                }
            }, 100);

        }

    }


    //Should Add Blazor Dep for this if we want to do that.
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