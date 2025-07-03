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

            // Group messages by domain
            var messagesByDomain = _azureEventGridMessages
                .GroupBy(m => m.DomainName)
                .ToList();

            foreach (var domainGroup in messagesByDomain)
            {
                if (domainGroup.Key != null)
                {
                    // Event Domain pattern - generate domain + domain topics
                    var domainName = domainGroup.Key;
                    var domainData = Terraform.azurerm_eventgrid_domain.domain(domainName);
                    
                    // Generate the domain resource
                    builder.AddResource(Terraform.azurerm_eventgrid_domain.type, domainData.refname, resource =>
                    {
                        resource.AddSetting("name", $"{domainName}");
                        resource.AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location);
                        resource.AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name);
                        resource.AddSetting("input_schema", "CloudEventSchemaV1_0");
                    });

                    // Generate domain topic resources for each unique topic in the domain
                    var uniqueTopics = new HashSet<string>();
                    foreach (var message in domainGroup)
                    {
                        if (uniqueTopics.Add(message.TopicName))
                        {
                            var domainTopicData = Terraform.azurerm_eventgrid_domain_topic.domainTopic(message);
                            builder.AddResource(Terraform.azurerm_eventgrid_domain_topic.type, domainTopicData.refname, resource =>
                            {
                                resource.AddSetting("name", message.TopicName);
                                resource.AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name);
                                resource.AddRawSetting("domain_name", $"{Terraform.azurerm_eventgrid_domain.type}.{domainData.refname}.name");
                            });
                        }
                    }
                }
                else
                {
                    // Custom Topic pattern - generate individual topics (backwards compatibility)
                    var uniqueTopics = new HashSet<string>();
                    foreach (var message in domainGroup)
                    {
                        if (uniqueTopics.Add(message.TopicName))
                        {
                            var topicData = Terraform.azurerm_eventgrid_topic.topic(message);
                            builder.AddResource(Terraform.azurerm_eventgrid_topic.type, topicData.refname, resource =>
                            {
                                resource.AddSetting("name", message.TopicName);
                                resource.AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location);
                                resource.AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name);
                                resource.AddSetting("input_schema", "CloudEventSchemaV1_0");
                            });
                        }
                    }
                }
            }

            return builder.Build();
        }
    }
}