using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
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
            return ExecutionContext.FindTemplateInstances<ITemplate>(TemplateRoles.Distribution.AzureFunctions.AzureFunctionEndpoint).Any()
                || ExecutionContext.FindTemplateInstances<ITemplate>(TemplateRoles.Distribution.AzureFunctions.AzureFunctionConsumer).Any();
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
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace('.', '-').ToKebabCase();

            var sb = new StringBuilder(128);
            sb.AppendLine($"param functionAppName string = '{sanitizedAppName}-${{uniqueString(resourceGroup().id)}}'");
            sb.AppendLine($"param appInsightsName string = 'app-insights-${{uniqueString(resourceGroup().id)}}'");
            sb.AppendLine($"param storageName string = 'storage${{uniqueString(resourceGroup().id)}}'");
            if (HasAzureServiceBus())
            {
                sb.AppendLine($"param serviceBusNamespaceName string = 'service-bus-${{uniqueString(resourceGroup().id)}}'");
            }

            sb.AppendLine();

            sb.AppendLine(new BicepResource("appInsights", "'Microsoft.Insights/components@2020-02-02'")
                .Set("name", "appInsightsName")
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

            sb.AppendLine(new BicepResource("storageAccount", "'Microsoft.Storage/storageAccounts@2022-09-01'")
                .Set("name", "storageName")
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

            var configVarMappings = new Dictionary<string, string>();

            if (HasAzureServiceBus())
            {
                configVarMappings["AzureServiceBus:ConnectionString"] = "serviceBusConnectionString";
            }

            foreach (var @event in _infrastructureEvents.OrderBy(x => x.InfrastructureComponent).ThenBy(x => string.Join(',', x.Properties.Select(x => $"{x.Key}:{x.Value}"))))
            {
                if (@event.InfrastructureComponent == Infrastructure.AzureServiceBus.QueueType)
                {
                    var varName = $"{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}Queue".ToPascalCase().ToCamelCase();
                    sb.AppendLine(new BicepResource(varName, "'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview'")
                        .WithExisting(@event.Properties[Infrastructure.AzureServiceBus.Property.External] == "true")
                        .Set("parent", "serviceBusNamespace")
                        .Set("name", $"'{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}'")
                        .Build());

                    var configName = @event.Properties[Infrastructure.AzureServiceBus.Property.ConfigurationName];
                    configVarMappings[configName] = varName + ".name";
                }
                else if (@event.InfrastructureComponent == Infrastructure.AzureServiceBus.TopicType)
                {
                    var varName = $"{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}Topic".ToPascalCase().ToCamelCase();
                    sb.AppendLine(new BicepResource(varName, "'Microsoft.ServiceBus/namespaces/topics@2021-06-01-preview'")
                        .WithExisting(@event.Properties[Infrastructure.AzureServiceBus.Property.External] == "true")
                        .Set("parent", "serviceBusNamespace")
                        .Set("name", $"'{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}'")
                        .Build());

                    var configName = @event.Properties[Infrastructure.AzureServiceBus.Property.ConfigurationName];
                    configVarMappings[configName] = varName + ".name";
                }
                else if (@event.InfrastructureComponent == Infrastructure.AzureServiceBus.SubscriptionType)
                {
                    var varName = $"{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}Subscription".ToPascalCase().ToCamelCase();
                    sb.AppendLine(new BicepResource(varName, "'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-06-01-preview'")
                        .Set("parent", $"{@event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName]}Topic".ToPascalCase().ToCamelCase())
                        .Set("name", $"'{varName.ToKebabCase()}'")
                        .Build());

                    var configName = @event.Properties[Infrastructure.AzureServiceBus.Property.ConfigurationName];
                    configVarMappings[configName] = varName + ".name";
                }
                else if (@event.InfrastructureComponent == Infrastructure.AzureEventGrid.TopicRegistered)
                {
                    var topicName = @event.Properties[Infrastructure.AzureEventGrid.Property.TopicName];
                    var keyConfigName = @event.Properties[Infrastructure.AzureEventGrid.Property.KeyConfig];
                    var endpointConfigName = @event.Properties[Infrastructure.AzureEventGrid.Property.EndpointConfig];
                    var varName = $"eventGridTopic{topicName}".ToPascalCase().ToCamelCase();

                    sb.AppendLine(new BicepResource(varName, "'Microsoft.EventGrid/topics@2021-12-01'")
                        .Set("name", $"'{topicName.ToKebabCase()}'")
                        .Set("location", "resourceGroup().location")
                        .Set("properties", "{}")
                        .Build());

                    configVarMappings[keyConfigName] = $"{varName}.listKeys().key1";
                    configVarMappings[endpointConfigName] = $"{varName}.properties.endpoint";
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
                                foreach (var request in _appSettingsRequests.OrderBy(x => x.Key))
                                {
                                    if (configVarMappings.TryGetValue(request.Key, out var varName))
                                    {
                                        // Use the mapped variable name for this app setting
                                        arr.Object(obj => obj
                                            .Set("name", $"'{request.Key}'")
                                            .Set("value", varName));
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

                            config.Block("cors", cors =>
                            {
                                cors.Array("allowedOrigins", allowedOrigins =>
                                {
                                    allowedOrigins.ScalarValue("https://portal.azure.com");
                                });
                            });

                            var template = GetTemplate<ICSharpTemplate>("Intent.AzureFunctions.Isolated.Program", new TemplateDiscoveryOptions { TrackDependency = false, ThrowIfNotFound = false });
                            if (template != null && template.Project.GetProject().TryGetMaxNetAppVersion(out var version))
                            {
                                config.Set("netFrameworkVersion", $"'v{version.Major}.{version.Minor}'");
                            }
                        });
                })
                .Build());

            return sb.ToString();
        }
    }
}