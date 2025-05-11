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

namespace Intent.Modules.IaC.Terraform.Templates.ResourceGroupTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ResourceGroupTfTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.ResourceGroupTfTemplate";
        
        private bool _hasAzureServiceBus;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ResourceGroupTfTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            if (@event.InfrastructureComponent is Infrastructure.AzureServiceBus.QueueType or
                Infrastructure.AzureServiceBus.TopicType or
                Infrastructure.AzureServiceBus.SubscriptionType)
            {
                _hasAzureServiceBus = true;
            }
        }
        
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"main",
                fileExtension: "tf",
                relativeLocation: "terraform/01-resource-group"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var builder = new TerraformFileBuilder()
                .AddTerraformConfig(terraform =>
                {
                    terraform.AddBlock("required_providers", providers =>
                    {
                        providers.AddObject("azurerm", azurerm =>
                        {
                            azurerm.AddSetting("source", "hashicorp/azurerm");
                            azurerm.AddSetting("version", "~> 3.0");
                        });
                        providers.AddObject("random", random =>
                        {
                            random.AddSetting("source", "hashicorp/random");
                            random.AddSetting("version", "~> 3.0");
                        });
                    });
                })

                // Provider block
                .AddProvider("azurerm", provider => { provider.AddBlock("features"); })

                // Variable blocks
                .AddVariable("resource_group_name", variable => { variable.AddRawSetting("type", "string"); })
                .AddVariable("resource_group_location", variable => { variable.AddRawSetting("type", "string"); })

                // Random string resource
                .AddResource("random_string", "unique", resource =>
                {
                    resource.AddSetting("length", 8);
                    resource.AddSetting("special", false);
                    resource.AddSetting("upper", false);
                })

                // Locals block
                .AddLocals(locals =>
                {
                    locals.AddSetting("app_insights_name", "app-insights-${random_string.unique.result}");
                    locals.AddRawSetting("location", "azurerm_resource_group.rg.location");
                })

                // Resource group with comment
                .AddComment("Resource Group")
                .AddResource("azurerm_resource_group", "rg", resource =>
                {
                    resource.AddRawSetting("name", "var.resource_group_name");
                    resource.AddRawSetting("location", "var.resource_group_location");
                })

                // Application Insights with comment
                .AddComment("Application Insights")
                .AddResource("azurerm_application_insights", "app_insights", resource =>
                {
                    resource.AddRawSetting("name", "local.app_insights_name");
                    resource.AddRawSetting("location", "local.location");
                    resource.AddRawSetting("resource_group_name", "var.resource_group_name");
                    resource.AddSetting("application_type", "web");
                });

            if (_hasAzureServiceBus)
            {
                builder.AddComment("Azure Service Bus");
                var localBuilder = builder.GetElementBuilders().OfType<TerraformBlockBuilder>().FirstOrDefault(x => x.BlockName == "locals");
                localBuilder?.AddSetting("service_bus_namespace_name", "service-bus-${random_string.unique.result}");

                builder.AddResource("azurerm_servicebus_namespace", "service_bus", resource => resource
                    .AddRawSetting("name", "local.service_bus_namespace_name")
                    .AddRawSetting("location", "var.resource_group_location")
                    .AddRawSetting("resource_group_name", "var.resource_group_name")
                    .AddSetting("sku", "Standard"));
            }

            // Output blocks
            builder.AddOutput("resource_group_name", output => { output.AddRawSetting("value", "var.resource_group_name"); })
                .AddOutput("resource_group_location", output => { output.AddRawSetting("value", "var.resource_group_location"); })
                .AddOutput("app_insights_name", output => { output.AddRawSetting("value", "azurerm_application_insights.app_insights.name"); });

            if (_hasAzureServiceBus)
            {
                builder.AddOutput("service_bus_namespace_name", output => { output.AddRawSetting("value", "azurerm_servicebus_namespace.service_bus.name"); });
            }

            string result = builder.Build();
            return result;
        }
    }
}