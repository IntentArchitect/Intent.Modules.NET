using Intent.Eventing.AzureEventGrid.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Integration.IaC.Shared.AzureEventGrid;

internal class AzureEventGridMessage
{
    public AzureEventGridMessage(string ApplicationId, string ApplicationName, MessageModel MessageModel, AzureEventGridMethodType MethodType)
    {
        this.ApplicationId = ApplicationId;
        this.ApplicationName = ApplicationName;
        this.MessageModel = MessageModel;
        this.MethodType = MethodType;
        TopicName = GetTopicName(MessageModel);
        var baseConfigName = GetTopicConfigurationBaseName(MessageModel);
        TopicConfigurationSourceName = $"{baseConfigName}:Source"; 
        TopicConfigurationKeyName = $"{baseConfigName}:Key";
        TopicConfigurationEndpointName = $"{baseConfigName}:Endpoint";
        TopicSubscriptionConfigurationName = MethodType == AzureEventGridMethodType.Subscribe  
            ? $"{baseConfigName}Subscription".ToPascalCase() : null;
        DomainName = MessageModel.InternalElement.Package.IsEventingPackageModel() 
            ? new EventingPackageModel(MessageModel.InternalElement.Package).GetEventDomain()?.DomainName() 
            : null;
    }

    public MessageModel MessageModel { get; init; }
    
    public string ApplicationId { get; init; }
    public string ApplicationName { get; init; }
    public AzureEventGridMethodType MethodType { get; init; }
    public string TopicName { get; init; }
    public string TopicConfigurationSourceName { get; init; }
    public string TopicConfigurationKeyName { get; init; }
    public string TopicConfigurationEndpointName { get; init; }
    public string? TopicSubscriptionConfigurationName { get; init; }
    public string? DomainName { get; init; }

    private string GetTopicConfigurationBaseName(MessageModel messageModel)
    {
        var topic = GetTopicName(messageModel);
        const string prefix = "EventGrid:Topics:";
        return $"{prefix}{topic.ToPascalCase()}";
    }

    private string GetTopicName(MessageModel messageModel)
    {
        return messageModel.GetAzureEventGrid()?.TopicName() ?? "NO TOPIC";
    }
    
    public string GetModelTypeName(IntentTemplateBase template)
    {
        return template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", MessageModel);
    }

    public string GetSubscriberTypeName<T>(IntentTemplateBase<T> template)
    {
        return $"{template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventHandlerInterface")}<{GetModelTypeName(template)}>";
    }
}