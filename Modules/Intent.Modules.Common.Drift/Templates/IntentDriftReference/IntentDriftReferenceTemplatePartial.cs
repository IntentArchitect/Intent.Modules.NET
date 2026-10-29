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

namespace Intent.Modules.Common.Drift.Templates.IntentDriftReference
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentDriftReferenceTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Common.Drift.IntentDriftReference";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentDriftReferenceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // The reference covers every designer the run's templates contribute, so its text is built once they are all registered.
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("intent-drift", relativeLocation: "sync-model/references")
                .AfterBuild(file => file.FromMarkdown(ModelSyncGuidance.IntentDrift(ExecutionContext, ModelSyncGuidance.Designers(ExecutionContext))));
        }

        /// <summary>Written once some module contributes a designer to the drift check.</summary>
        [IntentManaged(Mode.Ignore)]
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ModelSyncGuidance.Designers(ExecutionContext).Count > 0;
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}