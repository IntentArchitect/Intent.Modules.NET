using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class BlazorSkillSampleFilesStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations.BlazorSkillSampleFilesStaticContentTemplateRegistration";

        public BlazorSkillSampleFilesStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "SkillSamples";

        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (application.InstalledModules.Any(x => x.ModuleId == "Intent.Blazor.Components.MudBlazor"))
            {
                return;
            }

            base.Register(registry, application);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };
    }
}
