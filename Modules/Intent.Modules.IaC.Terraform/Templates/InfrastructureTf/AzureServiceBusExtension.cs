using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.IaC.Terraform.Templates.InfrastructureTf;

internal class AzureServiceBusExtension
{
    private readonly List<InfrastructureRegisteredEvent> _events = [];

    public void ProcessEvent(InfrastructureRegisteredEvent @event)
    {
        if (@event.InfrastructureComponent is Infrastructure.AzureServiceBus.QueueType or
            Infrastructure.AzureServiceBus.TopicType or
            Infrastructure.AzureServiceBus.SubscriptionType)
        {
            _events.Add(@event);    
        }
    }

    private bool HasAzureServiceBus()
    {
        return _events.Count != 0;
    }

    public void ApplyAzureServiceBus(TerraformFileBuilder builder, Dictionary<string, string> configVarMappings)
    {
        if (!HasAzureServiceBus())
        {
            return;
        }

        var localBuilder = builder.GetElementBuilders().OfType<TerraformBlockBuilder>().FirstOrDefault(x => x.BlockName == "locals");
        localBuilder?.AddSetting("service_bus_namespace_name", "service-bus-${random_string.unique.result}");
        
        builder.AddResource("azurerm_servicebus_namespace", "service_bus", resource => resource
            .AddRawSetting("name", "local.service_bus_namespace_name")
            .AddRawSetting("location", "azurerm_resource_group.rg.location")
            .AddRawSetting("resource_group_name", "azurerm_resource_group.rg.name")
            .AddSetting("sku", "Standard"));

        configVarMappings["AzureServiceBus:ConnectionString"] = "azurerm_servicebus_namespace.service_bus.default_primary_connection_string";

        foreach (var @event in _events)
        {
            switch (@event.InfrastructureComponent)
            {
                case Infrastructure.AzureServiceBus.QueueType:
                {
                    var name = @event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName];
                    var varName = $"{name}Queue".ToPascalCase().ToSnakeCase();
                    builder.AddResource("azurerm_servicebus_queue", varName, resource => resource
                        .AddSetting("name", name)
                        .AddRawSetting("namespace_id", "azurerm_servicebus_namespace.service_bus.id"));
                    
                    var configName = @event.Properties[Infrastructure.AzureServiceBus.Property.ConfigurationName];
                    configVarMappings[configName] = $"azurerm_servicebus_queue.{varName}.id";
                }
                    break;
                case Infrastructure.AzureServiceBus.TopicType:
                {
                    var name = @event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName];
                    var varName = $"{name}Topic".ToPascalCase().ToSnakeCase();
                    builder.AddResource("azurerm_servicebus_topic", varName, resource => resource
                        .AddSetting("name", name)
                        .AddRawSetting("namespace_id", "azurerm_servicebus_namespace.service_bus.id"));
                    
                    var configName = @event.Properties[Infrastructure.AzureServiceBus.Property.ConfigurationName];
                    configVarMappings[configName] = $"azurerm_servicebus_topic.{varName}.id";
                }
                    break;
                case Infrastructure.AzureServiceBus.SubscriptionType:
                {
                    var name = @event.Properties[Infrastructure.AzureServiceBus.Property.QueueOrTopicName];
                    var varName = $"{name}Subscription".ToPascalCase().ToSnakeCase();
                    builder.AddResource("azurerm_servicebus_subscription", varName, resource => resource
                        .AddSetting("name", name)
                        .AddRawSetting("topic_id", $"azurerm_servicebus_topic.{$"{name}Topic".ToPascalCase().ToSnakeCase()}.id"));
                    
                    var configName = @event.Properties[Infrastructure.AzureServiceBus.Property.ConfigurationName];
                    configVarMappings[configName] = $"azurerm_servicebus_subscription.{varName}.id";
                }
                    break;
            }
        }
    }
}