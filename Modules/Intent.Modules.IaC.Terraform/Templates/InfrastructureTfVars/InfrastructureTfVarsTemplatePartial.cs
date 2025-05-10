using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.InfrastructureTfVars
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class InfrastructureTfVarsTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.IaC.Terraform.InfrastructureTfVarsTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public InfrastructureTfVarsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"terraform",
                fileExtension: "tfvars",
                relativeLocation: "terraform/02-infrastructure"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace('.', '-').ToKebabCase();
            var keysWithDefaults = new Dictionary<string, string>
            {
                { "app_insights_name", @"""" },
                { "resource_group_location", @"""East US""" },
                { "resource_group_name", $@"""rg-{sanitizedAppName}""" }
            };

            return this.MergeKeyValuePairs(keysWithDefaults);
        }
    }
}