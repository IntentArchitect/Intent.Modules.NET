using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class ThemeStylesheetsStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations.ThemeStylesheetsStaticContentTemplateRegistration";

        public ThemeStylesheetsStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "ThemeStylesheets";

        [IntentIgnore]
        protected override OverwriteBehaviour GetDefaultOverrideBehaviour(IOutputTarget outputTarget)
        {
            return OverwriteBehaviour.OverwriteDisabled;
        }

        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (application.Settings.GetBlazor().UseCustomStylesheets())
            {
                return;
            }

            base.Register(registry, application);
        }
    }
}
