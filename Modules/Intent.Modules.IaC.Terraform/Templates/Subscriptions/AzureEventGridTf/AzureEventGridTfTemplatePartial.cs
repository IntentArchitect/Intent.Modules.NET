using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IaC.Terraform.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Integration.IaC.Shared;
using Intent.Modules.Integration.IaC.Shared.AzureEventGrid;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.Subscriptions.AzureEventGridTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureEventGridTfTemplate : IntentTemplateBase<ApplicationInfo>
    {
        private readonly string _sanitizedApplicationName;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.Subscriptions.AzureEventGridTf";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureEventGridTfTemplate(IOutputTarget outputTarget, ApplicationInfo model) : base(TemplateId, outputTarget, model)
        {
            _sanitizedApplicationName = Model.Name.Replace('.', '-').ToKebabCase();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: _sanitizedApplicationName,
                fileExtension: "tf"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var builder = new TerraformFileBuilder();

            var appIdRef = $"{_sanitizedApplicationName.ToSnakeCase()}_id";
            builder.AddVariable(appIdRef, var => var.AddRawSetting("type", "string"));

            var topicSubscriptions = Intent.Modules.Integration.IaC.Shared.AzureEventGrid.IntegrationManager.Instance.GetAggregatedAzureEventGridSubscriptions(Model.Id)
                .GroupBy(k => k.SubscriptionItem.TopicName);

            // Group subscriptions by domain to handle both patterns
            var subscriptionsByDomain = topicSubscriptions
                .SelectMany(ts => ts)
                .GroupBy(s => s.SubscriptionItem.DomainName)
                .ToList();

            var createdDomainDataSources = new HashSet<string>();

            foreach (var domainGroup in subscriptionsByDomain)
            {
                if (domainGroup.Key != null)
                {
                    // Event Domain pattern
                    var domainName = domainGroup.Key;
                    
                    // Create domain data source once per domain
                    if (createdDomainDataSources.Add(domainName))
                    {
                        var domainData = Terraform.azurerm_eventgrid_domain.domainData(domainName);
                        builder.AddData(Terraform.azurerm_eventgrid_domain.type, domainData.refname, data =>
                        {
                            data.AddSetting("name", $"{domainName}-domain");
                            data.AddRawSetting("resource_group_name", "var.resource_group_name");
                        });
                    }

                    // Group by topic within domain
                    var topicGroups = domainGroup.GroupBy(s => s.SubscriptionItem.TopicName);
                    foreach (var topicGroup in topicGroups)
                    {
                        var subscription = topicGroup.First();
                        var message = subscription.SubscriptionItem;
                        var domainTopicData = Terraform.azurerm_eventgrid_domain_topic.domainTopicData(message);
                        var domainData = Terraform.azurerm_eventgrid_domain.domainData(domainName);

                        // Create domain topic data source
                        builder.AddData(Terraform.azurerm_eventgrid_domain_topic.type, domainTopicData.refname, data =>
                        {
                            data.AddSetting("name", message.TopicName);
                            data.AddRawSetting("domain_name", $"data.azurerm_eventgrid_domain.{domainData.refname}.name");
                            data.AddRawSetting("resource_group_name", "var.resource_group_name");
                        });

                        // Create event subscription for domain topic
                        builder.AddResource(Terraform.azurerm_eventgrid_event_subscription.type, $"{_sanitizedApplicationName.ToSnakeCase()}_{message.TopicName.ToSnakeCase()}", resource =>
                        {
                            resource.AddSetting("name", AzureHelper.EnsureValidLength(Model.Name, 64));
                            resource.AddRawSetting("scope", domainTopicData.id);
                            resource.AddSetting("event_delivery_schema", "CloudEventSchemaV1_0");
                            resource.AddBlock("azure_function_endpoint", b =>
                            {
                                b.AddSetting("function_id", $"${{var.{appIdRef}}}/functions/{GetFunctionName(subscription.EventHandlerModel)}");
                                b.AddSetting("max_events_per_batch", 1);
                                b.AddSetting("preferred_batch_size_in_kilobytes", 64);
                            });

                            resource.AddSetting("included_event_types",
                                topicGroup.Select(s => GetNamespace(s.SubscriptionItem.MessageModel) + $".{s.SubscriptionItem.MessageModel.Name.ToPascalCase()}"));
                        });
                    }
                }
                else
                {
                    // Custom Topic pattern (backwards compatibility)
                    var topicGroups = domainGroup.GroupBy(s => s.SubscriptionItem.TopicName);
                    foreach (var topicGroup in topicGroups)
                    {
                        var subscription = topicGroup.First();
                        var message = subscription.SubscriptionItem;
                        var topicData = Terraform.azurerm_eventgrid_topic.topicData(message);

                        builder.AddData(Terraform.azurerm_eventgrid_topic.type, topicData.refname, data =>
                        {
                            data.AddSetting("name", subscription.SubscriptionItem.TopicName);
                            data.AddRawSetting("resource_group_name", "var.resource_group_name");
                        });

                        builder.AddResource(Terraform.azurerm_eventgrid_event_subscription.type, $"{_sanitizedApplicationName.ToSnakeCase()}_{message.TopicName.ToSnakeCase()}", resource =>
                        {
                            resource.AddSetting("name", AzureHelper.EnsureValidLength(Model.Name, 64));
                            resource.AddRawSetting("scope", topicData.id);
                            resource.AddSetting("event_delivery_schema", "CloudEventSchemaV1_0");
                            resource.AddBlock("azure_function_endpoint", b =>
                            {
                                b.AddSetting("function_id", $"${{var.{appIdRef}}}/functions/{GetFunctionName(subscription.EventHandlerModel)}");
                                b.AddSetting("max_events_per_batch", 1);
                                b.AddSetting("preferred_batch_size_in_kilobytes", 64);
                            });

                            resource.AddSetting("included_event_types",
                                topicGroup.Select(s => GetNamespace(s.SubscriptionItem.MessageModel) + $".{s.SubscriptionItem.MessageModel.Name.ToPascalCase()}"));
                        });
                    }
                }
            }

            return builder.Build();
        }

        // Keep GetNamespace() in sync with:
        // - Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage.IntegrationEventMessageTemplate
        private static string GetNamespace(MessageModel model)
        {
            var classNamespace = model.InternalElement.Package.Name.ToCSharpNamespace();
            var extendedNamespace = model.GetParentFolders().Where(x =>
                {
                    if (string.IsNullOrWhiteSpace(x.Name))
                    {
                        return false;
                    }

                    return !x.HasFolderOptions() || x.GetFolderOptions().NamespaceProvider();
                })
                .Select(x => x.Name);
            var completeNamespace = string.Join(".", classNamespace.Split(".").Concat(extendedNamespace));
            return completeNamespace;
        }

        // Keep GetFunctionName() in sync with:
        // - Intent.Modules.AzureFunctions.AzureEventGrid.Templates.AzureFunctionConsumer.AzureFunctionConsumerTemplate
        private string GetFunctionName(IntegrationEventHandlerModel model)
        {
            var functionName = $"{model.Name.RemoveSuffix("Handler")}Consumer";

            if (!SimpleFunctionNames())
            {
                var path = string.Join("_", model.GetParentFolderNames());
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return $"{path}_{functionName}";
                }
            }

            return functionName;
        }

        private bool SimpleFunctionNames()
        {
            const string azureFunctionsSettingsGroup = "90437e3f-cb10-4e44-b229-cc30c4807bea";
            const string simpleFunctionNamesSetting = "ff298d6c-705b-41d9-9286-be85480a0abd";

            var value = ExecutionContext.GetApplicationConfig(Model.Id).ModuleSetting
                .FirstOrDefault(x => x.Id == azureFunctionsSettingsGroup)?.GetSetting(simpleFunctionNamesSetting)?.Value;
            bool.TryParse(value, out var result);
            return result;
        }
    }
}