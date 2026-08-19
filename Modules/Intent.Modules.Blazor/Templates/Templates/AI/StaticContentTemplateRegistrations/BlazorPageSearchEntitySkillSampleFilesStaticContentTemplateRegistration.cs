using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Templates.Templates.AI.BlazorPageSearchEntitySkill;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.StaticContentTemplateRegistrations
{
    public class BlazorPageSearchEntitySkillSampleFilesStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.StaticContentTemplateRegistrations.BlazorPageSearchEntitySkillSampleFilesStaticContentTemplateRegistration";

        public BlazorPageSearchEntitySkillSampleFilesStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "ComponentSkillSamples/blazor-page-search-entity";

        [IntentIgnore]
        public override string RelativeOutputPathPrefix => "blazor-page-search-entity";

        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };

        [IntentIgnore]
        protected override ITemplate CreateTemplate(IOutputTarget outputTarget, string fileFullPath, string fileRelativePath, OverwriteBehaviour defaultOverwriteBehaviour)
        {
            return new SkillSampleStaticContentTemplate(
                sourcePath: fileFullPath,
                relativeOutputPath: fileRelativePath,
                relativeOutputPathPrefix: RelativeOutputPathPrefix,
                templateId: TemplateId,
                outputTarget: outputTarget,
                replacements: Replacements(outputTarget),
                overwriteBehaviour: defaultOverwriteBehaviour,
                fileConfigConfigurationUpdater: UpdateTemplateFileConfig,
                skillTemplateId: BlazorPageSearchEntitySkillTemplate.TemplateId);
        }

        [IntentIgnore]
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (application.InstalledModules.Any(module => module.ModuleId == "Intent.Blazor.Components.MudBlazor"))
            {
                return;
            }

            base.Register(registry, application);
        }
    }
}
