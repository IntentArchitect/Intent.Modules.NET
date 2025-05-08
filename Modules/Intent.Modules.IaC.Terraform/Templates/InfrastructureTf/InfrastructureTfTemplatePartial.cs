using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Constants;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.InfrastructureTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class InfrastructureTfTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.IaC.Terraform.InfrastructureTfTemplate";

        private readonly List<InfrastructureRegisteredEvent> _infrastructureEvents = [];
        private readonly List<AppSettingRegistrationRequest> _appSettingsRequests = [];

        private bool _hasAzureServiceBus;
        
        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public InfrastructureTfTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
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
                fileName: $"main",
                fileExtension: "tf",
                relativeLocation: "terraform/01-infrastructure"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace('.', '-').ToKebabCase();
            var builder = new TerraformFileBuilder();

            builder.AddTerraformConfig(terraform => terraform
                    .AddComment("Terraform configuration for example project")
                    .AddBlock("required_providers", block => block
                        .AddObject("azurerm", b => b
                            .AddSetting("source", "hashicorp/azurerm")
                            .AddSetting("version", "~> 3.0"))
                        .AddObject("random", b => b
                            .AddSetting("source", "hashicorp/random")
                            .AddSetting("version", "~> 3.0")))
                // .AddBackend("azurerm", backend =>
                // {
                //     backend
                //         .AddComment("Store state in Azure Storage")
                //         .AddSetting("resource_group_name", "terraform-state-rg")
                //         .AddSetting("storage_account_name", "tfstatestore")
                //         .AddSetting("container_name", "tfstate")
                //         .AddSetting("key", "infrastructure.tfstate");
                // })
            );

            builder.AddProvider("azurerm", provider => { provider.AddBlock("features"); });

            builder.AddResource("random_string", "unique", resource => resource
                .AddSetting("length", 8)
                .AddSetting("special", false)
                .AddSetting("upper", false));

            builder.AddLocals(local => local
                .AddSetting("function_app_name", $@"{sanitizedAppName}-${{random_string.unique.result}}")
                .AddSetting("app_insights_name", @"app-insights-${random_string.unique.result}")
                .AddSetting("storage_name", @"storage${random_string.unique.result}")
                .AddRawSetting("resource_group_name", @"azurerm_resource_group.rg.name")
                .AddRawSetting("location", @"azurerm_resource_group.rg.location"));

            builder.AddVariable("input_resource_group_name", v => v
                .AddRawSetting("type", "string")
                .AddSetting("default", "rg-azure-function-eventgrid"));
            builder.AddVariable("input_resource_group_location", v => v
                .AddRawSetting("type", "string")
                .AddSetting("default", "South Africa North"));

            builder.AddComment("Resource Group");
            
            builder.AddResource("azurerm_resource_group", "rg", resource => resource
                .AddRawSetting("name", "var.input_resource_group_name")
                .AddRawSetting("location", "var.input_resource_group_location"));

            builder.AddComment("Application Insights");

            builder.AddResource("azurerm_application_insights", "app_insights", resource => resource
                .AddRawSetting("name", "local.app_insights_name")
                .AddRawSetting("location", "local.location")
                .AddRawSetting("resource_group_name", "local.resource_group_name")
                .AddSetting("application_type", "web"));

            builder.AddComment("Hosting Plan (App Service Plan)");

            builder.AddResource("azurerm_service_plan", "function_plan", resource => resource
                .AddSetting("name", @"asp-${local.function_app_name}")
                .AddRawSetting("location", "local.location")
                .AddRawSetting("resource_group_name", "local.resource_group_name")
                .AddSetting("os_type", "Windows")
                .AddSetting("sku_name", "Y1"));

            builder.AddComment("Storage Account");

            builder.AddResource("azurerm_storage_account", "storage", resource => resource
                .AddRawSetting("name", "local.storage_name")
                .AddRawSetting("location", "local.location")
                .AddRawSetting("resource_group_name", "local.resource_group_name")
                .AddSetting("account_tier", "Standard")
                .AddSetting("account_replication_type", "LRS")
                .AddSetting("account_kind", "StorageV2"));

            // builder.AddComment("Event Grid Topics");
            //
            // builder.AddResource("azurerm_eventgrid_topic", "client_created_event", resource => resource
            //     .AddSetting("name", "client-created-event")
            //     .AddRawSetting("location", "local.location")
            //     .AddRawSetting("resource_group_name", "local.resource_group_name"));
            //
            // builder.AddResource("azurerm_eventgrid_topic", "specific_topic", resource => resource
            //     .AddSetting("name", "specific-topic")
            //     .AddRawSetting("location", "local.location")
            //     .AddRawSetting("resource_group_name", "local.resource_group_name"));
            
            builder.AddComment("Function App");

            builder.AddResource("azurerm_windows_function_app", "function_app", resource => resource
                .AddRawSetting("name", "local.function_app_name")
                .AddRawSetting("location", "local.location")
                .AddRawSetting("resource_group_name", "local.resource_group_name")
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
                        .AddRawSetting(@"""APPINSIGHTS_INSTRUMENTATIONKEY""", "azurerm_application_insights.app_insights.instrumentation_key")
                        .AddSetting(@"""AzureWebJobsStorage""",
                            "DefaultEndpointsProtocol=https;AccountName=${azurerm_storage_account.storage.name};AccountKey=${azurerm_storage_account.storage.primary_access_key};EndpointSuffix=core.windows.net")
                        // .AddRawSetting(@"""EventGrid:Topics:ClientCreatedEvent:Endpoint""", "azurerm_eventgrid_topic.client_created_event.endpoint")
                        // .AddRawSetting(@"""EventGrid:Topics:ClientCreatedEvent:Key""", "azurerm_eventgrid_topic.client_created_event.primary_access_key")
                        // .AddRawSetting(@"""EventGrid:Topics:SpecificTopic:Endpoint""", "azurerm_eventgrid_topic.specific_topic.endpoint")
                        // .AddRawSetting(@"""EventGrid:Topics:SpecificTopic:Key""", "azurerm_eventgrid_topic.specific_topic.primary_access_key")
                        .AddSetting(@"""FUNCTIONS_WORKER_RUNTIME""", "dotnet-isolated");

                    foreach (var request in _appSettingsRequests.OrderBy(x => x.Key))
                    {
                        var value = request.Value?.ToString();
                        appSettings.AddRawSetting($@"""{request.Key}""", value is null ? "null" : value == "" ? @"""""" : value);
                    }
                }));

            builder.AddComment("Output values needed for the second deployment");
            
            builder.AddOutput("function_app_id", output => output
                .AddRawSetting("value", "azurerm_windows_function_app.function_app.id"));

            // builder.AddOutput("client_created_event_topic_id", output => output
            //     .AddRawSetting("value", "azurerm_eventgrid_topic.client_created_event.id"));
            //
            // builder.AddOutput("specific_topic_id", output => output
            //     .AddRawSetting("value", "azurerm_eventgrid_topic.specific_topic.id"));

            builder.AddOutput("function_app_name", output => output
                .AddRawSetting("value", "azurerm_windows_function_app.function_app.name"));

            builder.AddOutput("resource_group_name", output => output
                .AddRawSetting("value", "azurerm_resource_group.rg.name"));
            
            return builder.Build();
        }
    }
}