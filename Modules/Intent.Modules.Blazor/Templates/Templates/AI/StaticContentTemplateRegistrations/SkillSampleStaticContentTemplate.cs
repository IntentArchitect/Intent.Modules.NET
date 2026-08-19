using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Templates;

namespace Intent.Modules.Blazor.Templates.Templates.StaticContentTemplateRegistrations
{
    /// <summary>
    /// A sample-file static content template that keeps its existing on-disk content once the
    /// sibling AI skill identified by "skillTemplateId" has been hand-edited (its
    /// ContentHashMatchesDisk is false), instead of overwriting it back to the bundled sample.
    /// TransformText runs after every template's AfterTemplateRegistration has completed
    /// application-wide, so the sibling's ContentHashMatchesDisk is guaranteed to be accurate by
    /// then - unlike GetTemplateFileConfig/UpdateTemplateFileConfig, which runs during template
    /// registration, before any AfterTemplateRegistration hook has executed.
    /// </summary>
    public class SkillSampleStaticContentTemplate : StaticContentTemplate
    {
        private readonly string _skillTemplateId;

        public SkillSampleStaticContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string relativeOutputPathPrefix,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements,
            OverwriteBehaviour overwriteBehaviour,
            Func<ITemplateFileConfig, StaticContentTemplate, ITemplateFileConfig> fileConfigConfigurationUpdater,
            string skillTemplateId)
            : base(sourcePath, relativeOutputPath, relativeOutputPathPrefix, templateId, outputTarget, replacements, overwriteBehaviour, fileConfigConfigurationUpdater)
        {
            _skillTemplateId = skillTemplateId;
        }

        public override string TransformText()
        {
            if (this.TryGetTemplate<IMarkdownFileBuilderTemplate>(_skillTemplateId, out var skillTemplate) &&
                !skillTemplate.ContentHashMatchesDisk &&
                this.TryGetExistingFileContent(out var existingContent))
            {
                return existingContent;
            }

            return base.TransformText();
        }
    }
}
