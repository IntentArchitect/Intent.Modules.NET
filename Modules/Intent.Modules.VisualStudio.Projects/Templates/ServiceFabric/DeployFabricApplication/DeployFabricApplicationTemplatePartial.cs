using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.DeployFabricApplication
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DeployFabricApplicationTemplate : IntentTemplateBase<ServiceFabricProjectModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.ServiceFabric.DeployFabricApplication";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DeployFabricApplicationTemplate(IOutputTarget outputTarget, ServiceFabricProjectModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public string Content { get; set; }

        public override void AfterTemplateRegistration()
        {
            if (!TryGetExistingFileContent(out var existingFileContent))
            {
                existingFileContent = TransformText().ReplaceLineEndings();
            }

            Content = existingFileContent;

            base.AfterTemplateRegistration();
        }

        public override string RunTemplate()
        {
            return Content;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"Deploy-FabricApplication",
                fileExtension: "ps1",
                relativeLocation: "Scripts"
            );
        }

    }
}