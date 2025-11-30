using System.Collections.Generic;
using Intent.Eventing.AzureEventGrid.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Integration.IaC.Shared.AzureEventGrid;

internal class AzureEventGridMessage : IHasStereotypes, IHasName, IHasFolder, IElementWrapper
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
        DomainConfigurationKeyName = DomainName != null ? $"EventGrid:Domains:{DomainName.ToPascalCase()}:Key" : null;
        DomainConfigurationEndpointName = DomainName != null ? $"EventGrid:Domains:{DomainName.ToPascalCase()}:Endpoint" : null;
        HasAzureEventGridStereotype = MessageModel.HasAzureEventGrid();
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
    public string? DomainConfigurationKeyName { get; set; }
    public string? DomainConfigurationEndpointName { get; set; }
    public bool HasAzureEventGridStereotype { get; init; }

    private string GetTopicConfigurationBaseName(MessageModel messageModel)
    {
        var topic = GetTopicName(messageModel);
        const string prefix = "EventGrid:Topics:";
        return $"{prefix}{topic.ToPascalCase()}";
    }
    
    private string GetTopicName(MessageModel messageModel)
    {
        return messageModel.GetAzureEventGrid()?.TopicName() ?? messageModel.Name.ToKebabCase();
    }
    
    public string GetModelTypeName(IntentTemplateBase template)
    {
        return template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", MessageModel);
    }

    public string GetSubscriberTypeName<T>(IntentTemplateBase<T> template)
    {
        return $"{template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventHandlerInterface")}<{GetModelTypeName(template)}>";
    }

    public IEnumerable<IStereotype> Stereotypes => MessageModel.Stereotypes;

    public string Name => MessageModel.Name;

    public FolderModel Folder => MessageModel.Folder;

    public IElement InternalElement => MessageModel.InternalElement;
}