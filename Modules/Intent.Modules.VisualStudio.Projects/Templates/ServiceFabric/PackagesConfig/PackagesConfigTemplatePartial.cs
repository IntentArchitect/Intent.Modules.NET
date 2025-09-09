using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.PackagesConfig
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class PackagesConfigTemplate : IntentTemplateBase<ServiceFabricProjectModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.ServiceFabric.PackagesConfig";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public PackagesConfigTemplate(IOutputTarget outputTarget, ServiceFabricProjectModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"packages",
                fileExtension: "config"
            );
        }

    }
}