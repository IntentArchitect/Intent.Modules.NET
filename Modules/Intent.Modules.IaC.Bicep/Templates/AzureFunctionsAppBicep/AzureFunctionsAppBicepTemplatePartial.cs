using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Bicep.Templates.AzureFunctionsAppBicep
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureFunctionsAppBicepTemplate : IntentTemplateBase<object>
    {
        private readonly List<InfrastructureRegisteredEvent> _infrastructureEvents = [];
        private readonly List<AppSettingRegistrationRequest> _appSettingsRequests = [];

        private bool _hasAzureServiceBus;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Bicep.AzureFunctionsAppBicepTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureFunctionsAppBicepTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<AppSettingRegistrationRequest>(Handle);
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            _infrastructureEvents.Add(@event);

            if (@event.InfrastructureComponent is Infrastructure.AzureServiceBus.QueueType or
                Infrastructure.AzureServiceBus.TopicType or
                Infrastructure.AzureServiceBus.SubscriptionType)
            {
                _hasAzureServiceBus = true;
            }
        }

        private void Handle(AppSettingRegistrationRequest request)
        {
            _appSettingsRequests.Add(request);
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.FindTemplateInstances<ITemplate>(TemplateRoles.Distribution.AzureFunctions.AzureFunctionEndpoint).Any();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"azure_functions",
                fileExtension: "bicep"
            );
        }

        private bool HasAzureServiceBus() => _hasAzureServiceBus;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.ToKebabCase();

            var sb = new StringBuilder(128);
            sb.AppendLine($"param functionAppName string = '{sanitizedAppName}-${{uniqueString(resourceGroup().id)}}'");
            if (HasAzureServiceBus())
            {
                sb.AppendLine($"param serviceBusNamespaceName string = '{sanitizedAppName}'");
            }

            sb.AppendLine();

            sb.AppendLine(new BicepResource("appInsights", "'Microsoft.Insights/components@2020-02-02'")
                .Set("name", "'app-insights-${functionAppName}'")
                .Set("location", "resourceGroup().location")
                .Set("kind", "'web'")
                .Block("properties", props =>
                {
                    props.Set("Application_Type", "'web'")
                        .Set("Request_Source", "'rest'");
                })
                .Build());

            sb.AppendLine(new BicepResource("functionAppHostingPlan", "'Microsoft.Web/serverfarms@2021-02-01'")
                .Set("name", "'asp-${functionAppName}'")
                .Set("location", "resourceGroup().location")
                .Block("sku", sku =>
                {
                    sku.Set("name", "'Y1'")
                        .Set("tier", "'Dynamic'");
                })
                .Build());

            sb.AppendLine(new BicepResource("storageAccount", "'Microsoft.Storage/storageAccounts@2021-02-01'")
                .Set("name", "'storage${uniqueString(resourceGroup().id)}'")
                .Set("location", "resourceGroup().location")
                .Set("kind", "'StorageV2'")
                .Block("sku", sku => { sku.Set("name", "'Standard_LRS'"); })
                .Build());

            if (HasAzureServiceBus())
            {
                sb.AppendLine(new BicepResource("serviceBusNamespace", "'Microsoft.ServiceBus/namespaces@2021-06-01-preview'")
                .Set("name", "serviceBusNamespaceName")
                .Set("location", "resourceGroup().location")
                .Block("sku", sku =>
                {
                    sku.Set("name", "'Standard'")
                        .Set("tier", "'Standard'");
                })
                .Build());
            }

            Dictionary<string, string> configVarMappings = new Dictionary<string, string>();

            foreach (var @event in _infrastructureEvents)
            {
                if (@event.InfrastructureComponent == Infrastructure.AzureServiceBus.QueueType)
                {
                    var varName = $"{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}Queue".ToPascalCase().ToCamelCase();
                    sb.AppendLine(new BicepResource(varName, "'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview'")
                        .Set("parent", "serviceBusNamespace")
                        .Set("name", $"'{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}'")
                        .Build());

                    var configName = @event.Properties[Infrastructure.AzureServiceBus.Property.ConfigurationName];
                    configVarMappings[configName] = varName;
                }
                else if (@event.InfrastructureComponent == Infrastructure.AzureServiceBus.TopicType)
                {
                    var varName = $"{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}Topic".ToPascalCase().ToCamelCase();
                    sb.AppendLine(new BicepResource(varName, "'Microsoft.ServiceBus/namespaces/topics@2021-06-01-preview'")
                        .Set("parent", "serviceBusNamespace")
                        .Set("name", $"'{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}'")
                        .Build());

                    var configName = @event.Properties[Infrastructure.AzureServiceBus.Property.ConfigurationName];
                    configVarMappings[configName] = varName;
                }
                else if (@event.InfrastructureComponent == Infrastructure.AzureServiceBus.SubscriptionType)
                {
                    var varName = $"{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}Subscription".ToPascalCase().ToCamelCase();
                    sb.AppendLine(new BicepResource(varName, "'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-06-01-preview'")
                        .Set("parent", $"{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}Topic".ToPascalCase().ToCamelCase())
                        .Set("name", $"'{varName.ToKebabCase()}'")
                        .Build());

                    var configName = @event.Properties[Infrastructure.AzureServiceBus.Property.ConfigurationName];
                    configVarMappings[configName] = varName;
                }
            }

            sb.AppendLine("var storageAccountKey = storageAccount.listKeys().keys[0].value");
            sb.AppendLine(
                "var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccountKey};EndpointSuffix=${environment().suffixes.storage}'");
            sb.AppendLine();

            if (HasAzureServiceBus())
            {
                sb.AppendLine("var serviceBusKey = listKeys(");
                sb.AppendLine("  '${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey',");
                sb.AppendLine("  serviceBusNamespace.apiVersion");
                sb.AppendLine(").primaryKey");
                sb.AppendLine(
                    "var serviceBusConnectionString = 'Endpoint=sb://${serviceBusNamespace.name}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=${serviceBusKey}'");
                sb.AppendLine();
            }

            sb.AppendLine(new BicepResource("functionApp", "'Microsoft.Web/sites@2021-02-01'")
                .Set("name", "functionAppName")
                .Set("location", "resourceGroup().location")
                .Set("kind", "'functionapp'")
                .Block("properties", props =>
                {
                    props.Set("serverFarmId", "functionAppHostingPlan.id")
                        .Block("siteConfig", config =>
                        {
                            config.Array("appSettings", arr =>
                            {
                                arr.Object(obj => obj
                                    .Set("name", "'APPINSIGHTS_INSTRUMENTATIONKEY'")
                                    .Set("value", "appInsights.properties.InstrumentationKey"));
                                arr.Object(obj => obj
                                    .Set("name", "'AzureWebJobsStorage'")
                                    .Set("value", "storageConnectionString"));
                                if (HasAzureServiceBus())
                                {
                                    arr.Object(obj => obj
                                        .Set("name", "'AzureServiceBus:ConnectionString'")
                                        .Set("value", "serviceBusConnectionString"));
                                }
                                foreach (var request in _appSettingsRequests)
                                {
                                    if (configVarMappings.TryGetValue(request.Key, out var varName))
                                    {
                                        // Use the mapped variable name for this app setting
                                        arr.Object(obj => obj
                                            .Set("name", $"'{request.Key}'")
                                            .Set("value", varName + ".name"));
                                    }
                                    else
                                    {
                                        // For other app settings, use the value as before
                                        arr.Object(obj => obj
                                            .Set("name", $"'{request.Key}'")
                                            .Set("value", $"'{request.Value?.ToString() ?? ""}'"));
                                    }
                                }
                            });
                        });
                })
                .Build());

            return sb.ToString();
        }
    }
}