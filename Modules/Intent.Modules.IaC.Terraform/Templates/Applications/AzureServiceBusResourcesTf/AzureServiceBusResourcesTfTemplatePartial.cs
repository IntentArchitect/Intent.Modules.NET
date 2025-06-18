using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.IaC.Shared;
using Intent.Modules.Integration.IaC.Shared.AzureServiceBus;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.Applications.AzureServiceBusResourcesTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureServiceBusResourcesTfTemplate : IntentTemplateBase<object>
    {
        private readonly List<AzureServiceBusItemBase> _azureServiceBusMessages;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.Applications.AzureServiceBusResourcesTf";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureServiceBusResourcesTfTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var apps = ExecutionContext.GetSolutionConfig()
                .GetApplicationReferences()
                .Select(app => ExecutionContext.GetSolutionConfig().GetApplicationConfig(app.Id))
                .ToArray();
            _azureServiceBusMessages = apps.SelectMany(app => IntegrationManager.Instance.GetAggregatedAzureServiceBusItems(app.Id)).ToList();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"azure-service-bus-resources",
                fileExtension: "tf"
            );
        }
        
        public override bool CanRunTemplate()
        {
            return _azureServiceBusMessages.Count > 0;
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

            var items = new HashSet<string>();
            foreach (var message in _azureServiceBusMessages)
            {
                if (items.Add(message.QueueOrTopicName))
                {
                    var resourceType = message.ChannelType == AzureServiceBusChannelType.Queue
                        ? Terraform.azurerm_servicebus_queue.type
                        : Terraform.azurerm_servicebus_topic.type;
                    var resourceName = message.ChannelType == AzureServiceBusChannelType.Queue
                        ? Terraform.azurerm_servicebus_queue.queue(message).refname
                        : Terraform.azurerm_servicebus_topic.topic(message).refname;
                    builder.AddResource(resourceType, resourceName, resource =>
                    {
                        resource.AddSetting("name", message.QueueOrTopicName);
                        resource.AddRawSetting("namespace_id", Terraform.azurerm_servicebus_namespace.service_bus.id);
                    });
                }

                if (message is { MethodType: AzureServiceBusMethodType.Subscribe, ChannelType: AzureServiceBusChannelType.Topic })
                {
                    builder.AddResource(Terraform.azurerm_servicebus_subscription.type, Terraform.azurerm_servicebus_subscription.subscription(message).refname, resource =>
                    {
                        var subscriptionName = AzureHelper.EnsureValidLength(message.ApplicationName.ToKebabCase(), 50);
                        resource.AddSetting("name", subscriptionName);
                        resource.AddRawSetting("topic_id", Terraform.azurerm_servicebus_topic.topic(message).id);
                        resource.AddSetting("max_delivery_count", 3);
                    });
                }
            }

            return builder.Build();
        }
    }
}