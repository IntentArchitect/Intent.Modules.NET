using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class UserMenuDefaultStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations.UserMenuDefaultStaticContentTemplateRegistration";

        public UserMenuDefaultStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "UserMenuDefault";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>();

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            // The Authentication module ships the real Mud AppUserMenu (account actions) to
            // Components/Account/Shared; defer to it. When Auth isn't installed, ship this no-op scaffold so
            // the model-generated Mud layout's injected <AppUserMenu/> still resolves.
            if (application.InstalledModules.Any(module => module.ModuleId == "Intent.Blazor.Authentication"))
            {
                return;
            }

            base.Register(registry, application);
        }
    }
}
