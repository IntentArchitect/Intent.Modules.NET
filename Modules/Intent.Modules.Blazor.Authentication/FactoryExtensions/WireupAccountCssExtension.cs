using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Authentication.Api;
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
            var securityType = application.MetadataManager.GetAuthenticationType(application.Id);

            // ux-account.css ships for every Authentication mode that has a LOCAL account UI - which is
            // all three of them. OIDC here is the resource-owner password flow (OidcAuthService posts
            // grant_type=password to connect/token) against a generated oidc-login page carrying the
            // same hand-rolled login-input-* form as Identity and JWT; it is NOT a redirect to an
            // external IdP. Gating this on !IsOidc() left OIDC applications with the markup and no
            // stylesheet.
            //
            // Only "None" has no account pages at all. It ships no ux-account.css, so linking one would
            // 404 on every page load.
            if (securityType.IsNone())
            {
                return;
            }

            // Deliberately MudBlazor-agnostic: AccountTheme and MudBlazorAccountTheme ship two different
            // ux-account.css variants under the same href, so whichever one was materialised is the one
            // that loads and a single link serves both.

            // AppRazorTemplate is host-scoped, and a multi-host application can have Blazor components
            // in more than one host - loop every instance instead of the singular, application-wide
            // lookup, which throws once a second Blazor host exists.
            var appRazorTemplates = application
                .FindTemplateInstances<IRazorFileTemplate>("Intent.Blazor.Templates.Server.AppRazorTemplate")
                .ToArray();

            if (appRazorTemplates.Length == 0)
            {
                Logging.Log.Warning("Unable to install ux-account.css. App.razor could not be found.");
                return;
            }

            foreach (var appRazorTemplate in appRazorTemplates)
            {
                var app = appRazorTemplate.RazorFile;

                app.AfterBuild(file =>
                {
                    var links = file.SelectHtmlElements("/html/head/link").ToList();

                    var accountCssLink = new HtmlElement("link", app)
                        .AddAttribute("rel", "stylesheet")
                        .AddAttribute("href", "ux-account.css");

                    // Anchor on the scoped-CSS bundle ({Project}.styles.css). AppRazorTemplate emits it
                    // unconditionally and last, which makes it the one link always present and never
                    // repositioned by another module, and inserting ABOVE it satisfies both orderings
                    // that matter:
                    //
                    //  - AFTER ux-mudblazor.css, because ux-account.css overrides the shared MudBlazor
                    //    page-header utility (.ux-gradient-primary) by cascade order rather than
                    //    !important. Linking it earlier silently reintroduces the full-bleed
                    //    frosted-banner bug on the account pages.
                    //  - BEFORE the bundle, so a page's own scoped .razor.css still wins over the
                    //    global sheet.
                    //
                    // Do NOT anchor on app.css: that link is emitted only when app.css is actually
                    // shipped (Intent.Blazor's TemplateHelper.ShipsAppCss), so it is absent from most
                    // applications.
                    var stylesBundle = links.FirstOrDefault(x =>
                        x.GetAttribute("href")?.Value?.EndsWith(".styles.css", StringComparison.OrdinalIgnoreCase) == true);

                    if (stylesBundle != null)
                    {
                        stylesBundle.AddAbove(accountCssLink);
                        return;
                    }

                    // Fallbacks for an App.razor head that has been restructured by hand, in descending
                    // order of how well each preserves the ordering above.
                    var fallbackAnchor = FindLink(links, "app.css")
                                         ?? FindLink(links, "ux-mudblazor.css")
                                         ?? FindLink(links, "ux-components.css");

                    if (fallbackAnchor != null)
                    {
                        fallbackAnchor.AddBelow(accountCssLink);
                        return;
                    }

                    Logging.Log.Warning("Unable to install ux-account.css. No stylesheet link was found in App.razor's head to anchor it to.");
                }, 100);
            }
        }

        [IntentIgnore]
        private static IHtmlElement FindLink(IEnumerable<IHtmlElement> links, string href)
        {
            return links.FirstOrDefault(x => x.HasAttribute("href", href));
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
