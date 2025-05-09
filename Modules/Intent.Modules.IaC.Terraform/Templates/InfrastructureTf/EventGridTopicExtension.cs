using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.IaC.Terraform.Templates.InfrastructureTf;

internal class EventGridTopicExtension
{
    private readonly List<EventGridTopic> _topics = [];

    private record EventGridTopic(string TopicName, string KeyConfigName, string EndpointConfigName);
    
    public EventGridTopicExtension()
    {
    }
    
    public void ProcessEvent(InfrastructureRegisteredEvent @event)
    {
        if (@event.InfrastructureComponent is not Infrastructure.AzureEventGrid.TopicRegistered)
        {
            return;
        }

        _topics.Add(new EventGridTopic(
            TopicName: @event.Properties[Infrastructure.AzureEventGrid.Property.TopicName].ToPascalCase(),
            KeyConfigName: @event.Properties[Infrastructure.AzureEventGrid.Property.KeyConfig],
            EndpointConfigName: @event.Properties[Infrastructure.AzureEventGrid.Property.EndpointConfig]));
    }

    public void ApplyTopics(TerraformFileBuilder builder, Dictionary<string, string> configVarMappings)
    {
        foreach (var topic in _topics)
        {
            var varName = $"eventGridTopic{topic.TopicName}".ToSnakeCase();

            builder.AddResource("azurerm_eventgrid_topic", varName, resource => resource
                .AddSetting("name", $"'{topic.TopicName.ToKebabCase()}'")
                .AddRawSetting("location", "local.location")
                .AddRawSetting("resource_group_name", "local.resource_group_name"));   
            
            configVarMappings[topic.KeyConfigName] = $"azurerm_eventgrid_topic.{varName}.id";
            configVarMappings[topic.EndpointConfigName] = $"azurerm_eventgrid_topic.{varName}.endpoint";
        }
    }

    public void ApplyOutput(TerraformFileBuilder builder)
    {
        foreach (var topic in _topics)
        {
            var varName = $"eventGridTopic{topic.TopicName}".ToSnakeCase();
            
            builder.AddOutput($"{varName}_id", output => output
                .AddRawSetting("value", $"azurerm_eventgrid_topic.{varName}.id"));
        }
    }
}