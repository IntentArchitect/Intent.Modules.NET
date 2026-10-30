using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.ConfigureDriftCheckSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ConfigureDriftCheckSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Services.Guardrails.Contracts.ConfigureDriftCheckSkill";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ConfigureDriftCheckSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "configure-drift-check")
                .FromMarkdown(ModelSyncGuidance.ConfigureDriftCheckSkill(ExecutionContext));
        }

        /// <summary>
        /// The skill covers every Guardrails designer, so when the Domain Guardrails module is installed it writes
        /// the file and this template doesn't.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ModelSyncGuidance.IsWriter(ExecutionContext, ModelSyncGuidance.ServicesModuleId);
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
