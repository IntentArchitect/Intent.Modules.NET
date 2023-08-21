using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ContinuousIntegration.AzurePipelines.Templates.AzurePipelines
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzurePipelinesTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ContinuousIntegration.AzurePipelines.azure-pipelines";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzurePipelinesTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"azure-pipelines",
                fileExtension: "yml",
                relativeLocation: "../",
                overwriteBehaviour: OverwriteBehaviour.OnceOff
            );
        }
    }
}