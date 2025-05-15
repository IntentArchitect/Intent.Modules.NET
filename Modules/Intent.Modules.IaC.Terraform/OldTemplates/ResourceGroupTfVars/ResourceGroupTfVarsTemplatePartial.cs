using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.ResourceGroupTfVars
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ResourceGroupTfVarsTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.ResourceGroupTfVarsTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ResourceGroupTfVarsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"terraform",
                fileExtension: "tfvars",
                relativeLocation: "terraform/01-resource-group"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace('.', '-').ToKebabCase();
            var keysWithDefaults = new Dictionary<string, string>
            {
                { "resource_group_name", $@"""rg-{sanitizedAppName}""" },
                { "resource_group_location", @"""East US""" }
            };
            return this.MergeKeyValuePairs(keysWithDefaults);
        }
    }
}