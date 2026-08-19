using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class ThemeToggleStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations.ThemeToggleStaticContentTemplateRegistration";

        public ThemeToggleStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "ThemeToggle";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        // UserMenu.razor.cs ships here and declares `namespace <#= Namespace #>Components.Layout`.
        { "Namespace", $"{outputTarget.GetNamespace()}." }
        };

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (!application.Settings.GetBlazor().EnableThemeToggle())
            {
                return;
            }

            // The MudBlazor module ships its own Mud-flavoured ThemeToggle (.razor + .razor.css) to the same
            // output path, so when it is installed we defer to it and skip ours to avoid two modules emitting
            // the same file. base.Register enumerates the folder; the .razor/.razor.css files inherit the
            // base Always overwrite behaviour (regenerated infrastructure).
            if (application.InstalledModules.Any(module => module.ModuleId == "Intent.Blazor.Components.MudBlazor"))
            {
                return;
            }

            base.Register(registry, application);
        }
    }
}
