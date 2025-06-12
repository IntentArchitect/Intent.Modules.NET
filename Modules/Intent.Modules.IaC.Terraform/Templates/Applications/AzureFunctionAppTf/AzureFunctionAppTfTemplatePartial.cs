using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IaC.Terraform.Api;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.IaC.Terraform.Templates.Applications.AzureFunctionAppTf;
using Intent.Modules.Integration.IaC.Shared;
using Intent.Modules.Integration.IaC.Shared.AzureEventGrid;
using Intent.Modules.Integration.IaC.Shared.AzureServiceBus;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.Applications.AzureFunctionAppTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureFunctionAppTfTemplate : IntentTemplateBase<ApplicationInfo>
    {
        private readonly string _sanitizedApplicationName;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.Applications.AzureFunctionAppTf";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureFunctionAppTfTemplate(IOutputTarget outputTarget, ApplicationInfo model) : base(TemplateId, outputTarget, model)
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

            const int randomSeedLength = 3;
            builder.AddResource("random_string", _sanitizedApplicationName, resource =>
            {
                resource.AddSetting("length", randomSeedLength)
                    .AddSetting("special", false)
                    .AddSetting("upper", false);
            });

            var appNameRef = $"{_sanitizedApplicationName}_app_name";
            var storageNameRef = $"{_sanitizedApplicationName.ToSnakeCase()}_storage_name";
            var localAppName = $"local.{appNameRef}";
            var localStorageName = $"local.{storageNameRef}";

            builder.AddLocals(locals =>
            {
                locals.AddSetting(appNameRef, _sanitizedApplicationName);
                locals.AddSetting(storageNameRef, GetStorageResourceName(_sanitizedApplicationName, randomSeedLength));
            });

            var functionPlanRef = $"{_sanitizedApplicationName}_function_plan";

            builder.AddComment("Hosting Plan (App Service Plan)");
            builder.AddResource("azurerm_service_plan", functionPlanRef, resource => resource
                .AddSetting("name", $"asp-${{{localAppName}}}")
                .AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location)
                .AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name)
                .AddSetting("os_type", "Windows")
                .AddSetting("sku_name", "Y1"));

            builder.AddComment("Storage Account");
            builder.AddResource("azurerm_storage_account", storageNameRef, resource =>
            {
                resource.AddRawSetting("name", localStorageName);
                resource.AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location);
                resource.AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name);
                resource.AddSetting("account_tier", "Standard");
                resource.AddSetting("account_replication_type", "LRS");
                resource.AddSetting("account_kind", "StorageV2");
            });

            var storageNameExpression = $"azurerm_storage_account.{storageNameRef}.name";
            var storagePrimaryAccessKeyExpression = $"azurerm_storage_account.{storageNameRef}.primary_access_key";
            var functionAppNameRef = $"{_sanitizedApplicationName}_function_app";

            builder.AddComment("Function App");
            builder.AddResource("azurerm_windows_function_app", functionAppNameRef, resource =>
            {
                resource.AddRawSetting("name", localAppName);
                resource.AddRawSetting("location", Terraform.azurerm_resource_group.main_rg.location);
                resource.AddRawSetting("resource_group_name", Terraform.azurerm_resource_group.main_rg.name);
                resource.AddRawSetting("service_plan_id", $"azurerm_service_plan.{functionPlanRef}.id");
                resource.AddRawSetting("storage_account_name", storageNameExpression);
                resource.AddRawSetting("storage_account_access_key", storagePrimaryAccessKeyExpression);
                resource.AddBlock("site_config", siteConfig =>
                {
                    siteConfig.AddBlock("application_stack", appStack =>
                    {
                        var versionValue = GetDotNetVersion();
                        if (versionValue is null)
                        {
                            return;
                        }
                        appStack.AddSetting("dotnet_version", $"v{versionValue.Value.Replace("net", string.Empty)}");
                    });
                    siteConfig.AddBlock("cors", cors => cors.AddSetting("allowed_origins", new[] { "https://portal.azure.com" }));
                });
                resource.AddObject("app_settings", appSettings =>
                {
                    appSettings.AddRawSetting(@"""APPINSIGHTS_INSTRUMENTATIONKEY""", Terraform.azurerm_application_insights.app_insights.instrumentation_key);
                    appSettings.AddSetting(@"""AzureWebJobsStorage""", $"DefaultEndpointsProtocol=https;AccountName=${{{storageNameExpression}}};AccountKey=${{{storagePrimaryAccessKeyExpression}}};EndpointSuffix=core.windows.net");

                    var versionValue = GetDotNetVersion();
                    var runtime = "dotnet";
                    if (IsVersionForIsolatedProcesses(versionValue?.Value.Replace("net", string.Empty)))
                    {
                        runtime = "dotnet-isolated";
                    }

                    appSettings.AddSetting(@"""FUNCTIONS_WORKER_RUNTIME""", runtime);

                    var appKeys = new HashSet<string>();
                    var azureServiceBusMessages = Integration.IaC.Shared.AzureServiceBus.IntegrationManager.Instance.GetAggregatedAzureServiceBusItems(Model.Id);
                    if (azureServiceBusMessages.Count != 0)
                    {
                        appSettings.AddRawSetting($@"""AzureServiceBus:ConnectionString""", Terraform.azurerm_servicebus_namespace.service_bus.default_primary_connection_string);
                    }
                    foreach (var message in azureServiceBusMessages)
                    {
                        if (appKeys.Add(message.QueueOrTopicConfigurationName))
                        {
                            var queueOrTopicExpression = message.ChannelType == AzureServiceBusChannelType.Queue
                                ? Terraform.azurerm_servicebus_queue.queue(message).name
                                : Terraform.azurerm_servicebus_topic.topic(message).name;
                            appSettings.AddRawSetting($@"""{message.QueueOrTopicConfigurationName}""", queueOrTopicExpression);
                        }
                        if (message is { MethodType: AzureServiceBusMethodType.Subscribe, ChannelType: AzureServiceBusChannelType.Topic })
                        {
                            appSettings.AddRawSetting($@"""{message.QueueOrTopicSubscriptionConfigurationName}""", Terraform.azurerm_servicebus_subscription.subscription(message).name);
                        }
                    }

                    var azureEventGridMessages = Integration.IaC.Shared.AzureEventGrid.IntegrationManager.Instance.GetAggregatedAzureEventGridMessages(Model.Id);
                    foreach (var message in azureEventGridMessages)
                    {
                        if (appKeys.Add(message.TopicConfigurationKeyName))
                        {
                            if (message.MethodType == AzureEventGridMethodType.Publish)
                            {
                                appSettings.AddRawSetting($@"""{message.TopicConfigurationSourceName}""", $@"""{message.TopicName.ToKebabCase()}""");
                            }
                            appSettings.AddRawSetting($@"""{message.TopicConfigurationKeyName}""", Terraform.azurerm_eventgrid_topic.topic(message).primary_access_key);
                            appSettings.AddRawSetting($@"""{message.TopicConfigurationEndpointName}""", Terraform.azurerm_eventgrid_topic.topic(message).endpoint);
                        }
                    }
                });
            });

            builder.AddComment("Output variables");
            builder.AddOutput($"{functionAppNameRef}_id", output =>
            {
                output.AddRawSetting("value", $"azurerm_windows_function_app.{functionAppNameRef}.id");
            });

            return builder.Build();
        }

        private static bool IsVersionForIsolatedProcesses(string? versionString)
        {
            if (string.IsNullOrWhiteSpace(versionString))
            {
                return false;
            }

            if (!Version.TryParse(versionString, out var version))
            {
                return false;
            }

            var minVersion = new Version(8, 0);
            return version >= minVersion;
        }

        private IElement? GetDotNetVersion()
        {
            const string visualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
            var designer = ExecutionContext.MetadataManager.GetDesigner(Model.Id, visualStudioDesignerId);

            const string csharpProjectType = "8e9e6693-2888-4f48-a0d6-0f163baab740";
            const string projectStereotype = "a490a23f-5397-40a1-a3cb-6da7e0b467c0";
            const string azureFuncVersionProperty = "9da51e12-529a-4592-9119-491d7bc9d5d5";

            var project = designer.GetElementsOfType(csharpProjectType).FirstOrDefault(IsAzureFunctionProject);
            if (project is null)
            {
                return null;
            }

            const string targetFrameworkProperty = "d53ab03c-90cf-4b6a-b733-73b6983ab603";
            var versionValue = project.GetStereotype(projectStereotype).GetProperty<IElement>(targetFrameworkProperty);
            return versionValue;

            bool IsAzureFunctionProject(IElement p) => (p.GetStereotype(projectStereotype)?.TryGetProperty(azureFuncVersionProperty, out var version) ?? false) && version?.Value != null;
        }

        private string GetStorageResourceName(string seed, int randomSeedLength)
        {
            const string storageSuffix = "storage";
            const int maxLength = 24;
            var context = seed.Replace("-", string.Empty).Replace("_", string.Empty).Replace(".", string.Empty);
            var calc = context.Length + randomSeedLength + storageSuffix.Length - maxLength;
            if (calc > 0)
            {
                context = context.Substring(0, maxLength - randomSeedLength - storageSuffix.Length - 1);
            }

            return $"{context}${{random_string.{_sanitizedApplicationName}.result}}{storageSuffix}";
        }
    }
}