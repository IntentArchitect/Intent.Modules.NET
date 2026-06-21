using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class UserMenuDefaultStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations.UserMenuDefaultStaticContentTemplateRegistration";

        public UserMenuDefaultStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "UserMenuDefault";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>();

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            // AppUserMenu is normally shipped by the Authentication module (the real account menu). When Auth
            // isn't installed, ship this no-op scaffold instead so a layout's <AppUserMenu/> still resolves.
            if (application.InstalledModules.Any(module => module.ModuleId == "Intent.Blazor.Authentication"))
            {
                return;
            }

            // MudBlazor apps generate their layout (and own the Components/Layout output target) via the
            // MudBlazor module, which ships its own no-Auth AppUserMenu default to that target. Defer to it —
            // this base/non-Mud registration only handles the plain (non-MudBlazor) stack.
            if (application.InstalledModules.Any(module => module.ModuleId == "Intent.Blazor.Components.MudBlazor"))
            {
                return;
            }

            base.Register(registry, application);
        }
    }
}
