using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations
{
    // Ships ux-account.css (the MudBlazor account/manage presentation layer) to wwwroot for the
    // MudBlazor ASP.NET Core Identity account pages — the counterpart to AccountTheme (which ships
    // the non-MudBlazor ux-account.css). Loads after ux-mudblazor.css so it can override the shared
    // page-header utility for account pages. Signature = Mode.Merge so the Software Factory preserves
    // the hand-set base class (AuthStaticContentTemplateRegistration) and the gated Register.
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class MudBlazorAccountThemeStaticContentTemplateRegistration : AuthStaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations.MudBlazorAccountThemeStaticContentTemplateRegistration";

        public MudBlazorAccountThemeStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "ThemeMudBlazor";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };

        [IntentIgnore]
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            var auth = application.MetadataManager.GetAuthenticationType(application.Id);
            var mudBlazorInstalled = application.InstalledModules.Any(im => im.ModuleId == "Intent.Blazor.Components.MudBlazor");

            // The account skin (ux-account.css + nav-drawer.js) is mode-independent — it tames the
            // shared page-header banner for account pages and drives centering. Ship it for any
            // MudBlazor app with a local account UI (Identity or JWT). OIDC redirects to an external
            // IdP (no local account UI) — the same !IsOidc() gate the account pages use.
            if (mudBlazorInstalled && !auth.IsSingleSignOnOpenIDConnect())
            {
                RegisterAuthStaticContent(registry, application);
            }
        }

        protected override void RegisterTemplate(ITemplateInstanceRegistry registry, IApplication application, Func<IOutputTarget, ITemplate> createTemplateInstance)
        {
            // TODO: JPS, so template not registered for now
        }
    }
}
