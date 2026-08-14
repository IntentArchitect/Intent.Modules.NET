using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class UserMenuDefaultClientStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations.UserMenuDefaultClientStaticContentTemplateRegistration";

        public UserMenuDefaultClientStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "UserMenuDefaultClient";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>();

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            // Two-project render modes only (InteractiveAuto / InteractiveWebAssembly). The interactive MainLayout
            // lives in the .Client (flat Layout/ folder) and injects <AppUserMenu/>, but the Authentication module
            // ships its real AppUserMenu to the server (Components/Account/Shared) and cannot target the .Client.
            // So ship this no-op scaffold to the .Client's Layout/ (beside MainLayout) so <AppUserMenu/> resolves
            // there. Single-project (InteractiveServer) is handled by UserMenuDefaultStaticContentTemplateRegistration
            // (ships to Components/Layout, beside the single-project MainLayout).
            if (application.GetSettings().GetBlazor().RenderMode().IsInteractiveServer())
            {
                return;
            }

            // Only needed when Authentication is installed: that's the only case <AppUserMenu/> is actually
            // referenced anywhere (the Authentication module ships the real one, server-side only). Without
            // Authentication there's no AppUserMenu reference to satisfy, so there's nothing to scaffold here.
            if (!application.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Authentication"))
            {
                return;
            }

            base.Register(registry, application);
        }
    }
}
