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

namespace Intent.Modules.IaC.Terraform.Templates.Subscriptions.AzureMainTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureMainTfTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.Subscriptions.AzureMainTf";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureMainTfTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"azure-main",
                fileExtension: "tf"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var builder = new TerraformFileBuilder();

            builder.AddTerraformConfig(conf =>
            {
                conf.AddBlock("required_providers", block => block
                    .AddObject("azurerm", b => b
                        .AddSetting("source", "hashicorp/azurerm")
                        .AddSetting("version", "~> 3.0"))
                );
            });

            builder.AddVariable("resource_group_name", var => var.AddRawSetting("type", "string"));
            
            return builder.Build();
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.TemplateExists(AzureEventGridTf.AzureEventGridTfTemplate.TemplateId);
        }
    }
}