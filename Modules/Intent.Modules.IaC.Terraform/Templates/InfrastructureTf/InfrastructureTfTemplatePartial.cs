using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
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

namespace Intent.Modules.IaC.Terraform.Templates.InfrastructureTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class InfrastructureTfTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.InfrastructureTfTemplate";

        private readonly List<AppSettingRegistrationRequest> _appSettingsRequests = [];
        private readonly EventGridTopicExtension _eventGridTopicExtension = new();
        private readonly AzureServiceBusExtension _azureServiceBusExtension = new();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public InfrastructureTfTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<AppSettingRegistrationRequest>(Handle);
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            _eventGridTopicExtension.ProcessEvent(@event);
            _azureServiceBusExtension.ProcessEvent(@event);
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
                fileName: $"main",
                fileExtension: "tf",
                relativeLocation: "terraform/02-infrastructure"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace('.', '-').ToKebabCase();
            var builder = new TerraformFileBuilder();

            builder.AddTerraformConfig(terraform => terraform
                    .AddBlock("required_providers", block => block
                        .AddObject("azurerm", b => b
                            .AddSetting("source", "hashicorp/azurerm")
                            .AddSetting("version", "~> 3.0"))
                        .AddObject("random", b => b
                            .AddSetting("source", "hashicorp/random")
                            .AddSetting("version", "~> 3.0"))));

            builder.AddProvider("azurerm", provider => provider.AddBlock("features"));

            builder.AddResource("random_string", "unique", resource => resource
                .AddSetting("length", 8)
                .AddSetting("special", false)
                .AddSetting("upper", false));

            builder.AddLocals(local => local
                .AddSetting("function_app_name", $@"{sanitizedAppName}-${{random_string.unique.result}}")
                .AddSetting("storage_name", @"storage${random_string.unique.result}"));

            builder.AddComment("Variables");
            
            builder.AddVariable("resource_group_name", v => v
                .AddRawSetting("type", "string"));
            builder.AddVariable("resource_group_location", v => v
                .AddRawSetting("type", "string"));
            builder.AddVariable("app_insights_name", v => v
                .AddRawSetting("type", "string"));
            _azureServiceBusExtension.ApplyVariables(builder);

            builder.AddComment("Hosting Plan (App Service Plan)");

            builder.AddResource("azurerm_service_plan", "function_plan", resource => resource
                .AddSetting("name", @"asp-${local.function_app_name}")
                .AddRawSetting("location", "var.resource_group_location")
                .AddRawSetting("resource_group_name", "var.resource_group_name")
                .AddSetting("os_type", "Windows")
                .AddSetting("sku_name", "Y1"));

            builder.AddComment("Storage Account");

            builder.AddResource("azurerm_storage_account", "storage", resource => resource
                .AddRawSetting("name", "local.storage_name")
                .AddRawSetting("location", "var.resource_group_location")
                .AddRawSetting("resource_group_name", "var.resource_group_name")
                .AddSetting("account_tier", "Standard")
                .AddSetting("account_replication_type", "LRS")
                .AddSetting("account_kind", "StorageV2"));

            var configVarMappings = new Dictionary<string, string>();
            _eventGridTopicExtension.ApplyAzureEventGrid(builder, configVarMappings);
            _azureServiceBusExtension.ApplyAzureServiceBus(builder, configVarMappings);

            builder.AddComment("Application Insights");

            builder.AddData("azurerm_application_insights", "app_insights", data => data
                .AddRawSetting("name", "var.app_insights_name")
                .AddRawSetting("resource_group_name", "var.resource_group_name")
            );
            
            builder.AddComment("Function App");

            builder.AddResource("azurerm_windows_function_app", "function_app", resource => resource
                .AddRawSetting("name", "local.function_app_name")
                .AddRawSetting("location", "var.resource_group_location")
                .AddRawSetting("resource_group_name", "var.resource_group_name")
                .AddRawSetting("service_plan_id", "azurerm_service_plan.function_plan.id")
                .AddRawSetting("storage_account_name", "azurerm_storage_account.storage.name")
                .AddRawSetting("storage_account_access_key", "azurerm_storage_account.storage.primary_access_key")
                .AddBlock("site_config", siteConfig => siteConfig
                    .AddBlock("application_stack", appStack =>
                    {
                        var template = GetTemplate<ICSharpTemplate?>("Intent.AzureFunctions.Isolated.Program", new TemplateDiscoveryOptions { TrackDependency = false, ThrowIfNotFound = false });
                        if (template != null && template.Project.GetProject().TryGetMaxNetAppVersion(out var version))
                        {
                            appStack.AddSetting("dotnet_version", $"v{version.Major}.{version.Minor}");
                        }
                    })
                    .AddBlock("cors", cors => cors
                        .AddSetting("allowed_origins", new[] { "https://portal.azure.com" })))
                .AddObject("app_settings", appSettings =>
                {
                    appSettings
                        .AddRawSetting(@"""APPINSIGHTS_INSTRUMENTATIONKEY""", "data.azurerm_application_insights.app_insights.instrumentation_key")
                        .AddSetting(@"""AzureWebJobsStorage""", "DefaultEndpointsProtocol=https;AccountName=${azurerm_storage_account.storage.name};AccountKey=${azurerm_storage_account.storage.primary_access_key};EndpointSuffix=core.windows.net");

                    foreach (var request in _appSettingsRequests.OrderBy(x => x.Key))
                    {
                        if (configVarMappings.TryGetValue(request.Key, out var varName))
                        {
                            appSettings.AddRawSetting($@"""{request.Key}""", varName);
                        }
                        else
                        {
                            var value = request.Value?.ToString();
                            appSettings.AddRawSetting($@"""{request.Key}""", value is null ? "null" : value == "" ? @"""""" : @$"""{value}""");
                        }
                    }
                }));

            builder.AddComment("Output values needed for the second deployment");

            builder.AddOutput("resource_group_name", output => output
                .AddRawSetting("value", "var.resource_group_name"));

            builder.AddOutput("function_app_id", output => output
                .AddRawSetting("value", "azurerm_windows_function_app.function_app.id"));

            return builder.Build();
        }
    }
}