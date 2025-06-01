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

namespace Intent.Modules.IaC.Terraform.Templates.Applications.AzureMainTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureMainTfTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.Applications.AzureMainTf";

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

            builder.AddTerraformConfig(terraform => terraform
                .AddBlock("required_providers", block => block
                    .AddObject("azurerm", b => b
                        .AddSetting("source", "hashicorp/azurerm")
                        .AddSetting("version", "~> 3.0"))));

            builder.AddProvider("azurerm", provider => provider.AddBlock("features"));

            builder.AddComment("Variables");
            builder.AddVariable("resource_group_name", var => var.AddRawSetting("type", "string"));
            builder.AddVariable("resource_group_location", var => var.AddRawSetting("type", "string"));

            builder.AddComment("Resource group");
            builder.AddResource(Terraform.azurerm_resource_group.type, Terraform.azurerm_resource_group.main_rg.refname, resource => resource
                .AddRawSetting("name", "var.resource_group_name")
                .AddRawSetting("location", "var.resource_group_location")
            );

            builder.AddComment("Workspace");
            builder.AddResource("azurerm_log_analytics_workspace", "workspace", resource => resource
                .AddSetting("name", "log-analytics-workspace-name")
                .AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location)
                .AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name)
                .AddSetting("sku", "PerGB2018")
                .AddSetting("retention_in_days", 30)
            );

            builder.AddComment("Application Insights");
            builder.AddResource(Terraform.azurerm_application_insights.type, Terraform.azurerm_application_insights.app_insights.refname, resource => resource
                .AddSetting("name", "app-insights")
                .AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location)
                .AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name)
                .AddSetting("application_type", "web")
                .AddRawSetting("workspace_id", "azurerm_log_analytics_workspace.workspace.id")
            );

            return builder.Build();
        }
    }
}