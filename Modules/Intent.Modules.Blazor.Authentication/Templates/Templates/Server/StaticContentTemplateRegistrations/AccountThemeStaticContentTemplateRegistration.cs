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
    // Ships ux-account.css (the non-MudBlazor account/manage presentation layer) to wwwroot,
    // mirroring how the MudBlazor module ships ux-mudblazor.css. Signature = Mode.Merge so the
    // Software Factory preserves the hand-set base class (AuthStaticContentTemplateRegistration).
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class AccountThemeStaticContentTemplateRegistration : AuthStaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations.AccountThemeStaticContentTemplateRegistration";

        public AccountThemeStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "Theme";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };

        [IntentIgnore]
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            // OIDC redirects to an external IdP — no local account UI. The account skin ships for
            // the modes with a local login (ASP.NET Core Identity + JWT), mirroring the !IsOidc()
            // gate the account pages use.
            var auth = application.MetadataManager.GetAuthenticationType(application.Id);
            if (auth.IsSingleSignOnOpenIDConnect())
            {
                return;
            }

            // The MudBlazor account pages use MudBlazor components + ux-mudblazor.css, not ux-account.css.
            if (application.InstalledModules.Any(im => im.ModuleId == "Intent.Blazor.Components.MudBlazor"))
            {
                return;
            }

            RegisterAuthStaticContent(registry, application);
        }

        protected override OverwriteBehaviour GetDefaultOverrideBehaviour(IOutputTarget outputTarget)
        {
            return OverwriteBehaviour.OverwriteDisabled;
        }
    }
}
