using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Components.MudBlazor.Templates.MudBlazorPageEditingEntitySkill;
using Intent.Modules.Blazor.Templates.Templates.StaticContentTemplateRegistrations;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations
{
    public class MudBlazorPageEditingEntitySkillSampleFilesStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations.MudBlazorPageEditingEntitySkillSampleFilesStaticContentTemplateRegistration";

        public MudBlazorPageEditingEntitySkillSampleFilesStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "ComponentSkillSamples/blazor-page-editing-entity";

        [IntentIgnore]
        public override string RelativeOutputPathPrefix => "blazor-page-editing-entity";

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
                skillTemplateId: MudBlazorPageEditingEntitySkillTemplate.TemplateId);
        }
    }
}
