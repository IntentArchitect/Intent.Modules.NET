using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.IaC.Terraform.Templates.InfrastructureTf;

internal class EventGridTopicExtension
{
    private readonly List<EventGridTopic> _topics = [];
    private readonly List<string> _subscriptions = [];

    private record EventGridTopic(string TopicName, string KeyConfigName, string EndpointConfigName);
    
    public EventGridTopicExtension()
    {
    }
    
    public void ProcessEvent(InfrastructureRegisteredEvent @event)
    {
        if (@event.InfrastructureComponent is Infrastructure.AzureEventGrid.TopicRegistered)
        {
            _topics.Add(new EventGridTopic(
                TopicName: @event.Properties[Infrastructure.AzureEventGrid.Property.TopicName],
                KeyConfigName: @event.Properties[Infrastructure.AzureEventGrid.Property.KeyConfig],
                EndpointConfigName: @event.Properties[Infrastructure.AzureEventGrid.Property.EndpointConfig]));
        }
        else if (@event.InfrastructureComponent is Infrastructure.AzureEventGrid.Subscription)
        {
            _subscriptions.Add(@event.Properties[Infrastructure.AzureEventGrid.Property.TopicName]);
        }
    }

    public void ApplyAzureEventGrid(TerraformFileBuilder builder, Dictionary<string, string> configVarMappings)
    {
        foreach (var topic in _topics)
        {
            var varName = $"eventGridTopic{topic.TopicName}".ToPascalCase().ToSnakeCase();

            builder.AddResource("azurerm_eventgrid_topic", varName, resource => resource
                .AddSetting("name", topic.TopicName.ToKebabCase())
                .AddRawSetting("location", "var.resource_group_location")
                .AddRawSetting("resource_group_name", "var.resource_group_name"));   
            
            configVarMappings[topic.KeyConfigName] = $"azurerm_eventgrid_topic.{varName}.primary_access_key";
            configVarMappings[topic.EndpointConfigName] = $"azurerm_eventgrid_topic.{varName}.endpoint";
        }
    }
}