using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.IaC.Terraform.Templates.Azure_Service_Bus;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.AzureServiceBus.AzureServiceBusResourcesTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureServiceBusResourcesTfTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.Azure Service Bus.AzureServiceBusResourcesTf";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureServiceBusResourcesTfTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"azure-service-bus-resources",
                fileExtension: "tf"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var builder = new TerraformFileBuilder();

            builder.AddResource(Terraform.azurerm_servicebus_namespace.type, Terraform.azurerm_servicebus_namespace.service_bus.refname, resource =>
            {
                resource.AddSetting("name", "azure-service-bus");
                resource.AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location);
                resource.AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name);
                resource.AddSetting("sku", "Standard");
            });

            var topics = AzureServiceBusManager.Instance.GetTopics(ExecutionContext.GetApplicationConfig().Id);
            foreach (var topic in topics)
            {
                
            }
            
            return builder.Build();
        }
    }
}