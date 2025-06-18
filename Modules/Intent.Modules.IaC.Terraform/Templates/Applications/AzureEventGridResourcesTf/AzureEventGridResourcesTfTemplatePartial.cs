using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.IaC.Shared;
using Intent.Modules.Integration.IaC.Shared.AzureEventGrid;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.Applications.AzureEventGridResourcesTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureEventGridResourcesTfTemplate : IntentTemplateBase<object>
    {
        private readonly List<AzureEventGridMessage> _azureEventGridMessages;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.Applications.AzureEventGridResourcesTf";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureEventGridResourcesTfTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var apps = ExecutionContext.GetSolutionConfig()
                .GetApplicationReferences()
                .Select(app => ExecutionContext.GetSolutionConfig().GetApplicationConfig(app.Id))
                .ToArray();
            _azureEventGridMessages = apps.SelectMany(app => IntegrationManager.Instance.GetPublishedAzureEventGridMessages(app.Id)).ToList();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"azure-event-grid-resources",
                fileExtension: "tf"
            );
        }

        public override bool CanRunTemplate()
        {
            return _azureEventGridMessages.Count > 0;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var builder = new TerraformFileBuilder();

            var items = new HashSet<string>();
            foreach (var message in _azureEventGridMessages)
            {
                if (items.Add(message.TopicName))
                {
                    builder.AddResource(Terraform.azurerm_eventgrid_topic.type, Terraform.azurerm_eventgrid_topic.topic(message).refname, resource =>
                    {
                        resource.AddSetting("name", message.TopicName);
                        resource.AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location);
                        resource.AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name);
                        resource.AddSetting("input_schema", "CloudEventSchemaV1_0");
                    });
                }
            }

            return builder.Build();
        }
    }
}