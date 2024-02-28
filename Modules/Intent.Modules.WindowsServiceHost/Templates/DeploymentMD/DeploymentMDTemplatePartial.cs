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

namespace Intent.Modules.WindowsServiceHost.Templates.DeploymentMD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DeploymentMDTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.WindowsServiceHost.DeploymentMD";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DeploymentMDTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public string ApplicationName()
        {
            return ExecutionContext.GetApplicationConfig().Name;
        }
        public string ExecutableName()
        {
            return ExecutionContext.GetApplicationConfig().Name.ToPascalCase();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"Deployment",
                fileExtension: "md"
            );
        }

    }
}