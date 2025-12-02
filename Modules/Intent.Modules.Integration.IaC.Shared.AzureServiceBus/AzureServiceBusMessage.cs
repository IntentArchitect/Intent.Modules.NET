using System;
using System.Collections.Generic;
using Intent.Eventing.AzureServiceBus.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Integration.IaC.Shared.AzureServiceBus;

internal record AzureServiceBusMessage : AzureServiceBusItemBase
{
    public AzureServiceBusMessage(string ApplicationId, string ApplicationName, MessageModel MessageModel, AzureServiceBusMethodType MethodType)
    {
        this.ApplicationId = ApplicationId;
        this.ApplicationName = ApplicationName;
        this.MessageModel = MessageModel;
        this.MethodType = MethodType;
        ChannelType = GetChannelType(MessageModel);
        QueueOrTopicName = GetQueueOrTopicName(MessageModel);
        QueueOrTopicConfigurationName = GetQueueOrTopicConfigurationName(MessageModel);
        QueueOrTopicSubscriptionConfigurationName = MethodType == AzureServiceBusMethodType.Subscribe && ChannelType == AzureServiceBusChannelType.Topic 
            ? $"{QueueOrTopicConfigurationName}Subscription".ToPascalCase() : null;
    }

    public MessageModel MessageModel { get; init; }

    private static string GetQueueOrTopicName(MessageModel message)
    {
        if (!message.HasAzureServiceBus())
        {
            return GetDefaultName();
        }
        
        var name = message.GetAzureServiceBus().Type().AsEnum() switch
        {
            MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Default => GetDefaultName(),
            MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => message.GetAzureServiceBus().QueueName(),
            MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => message.GetAzureServiceBus().TopicName(),
            var x => throw new ArgumentOutOfRangeException($"The Type {x} is not supported.")
        };

        return name;

        string GetDefaultName()
        {
            var resolvedName = message.Name;
            resolvedName = resolvedName.RemoveSuffix("IntegrationEvent", "Event", "Message");
            resolvedName = resolvedName.ToKebabCase();
            return resolvedName;
        }
    }

    private static string GetQueueOrTopicConfigurationName(MessageModel message)
    {
        const string prefix = "AzureServiceBus:";
        if (!message.HasAzureServiceBus())
        {
            return prefix + GetDefaultName();
        }
        
        var name = message.GetAzureServiceBus().Type().AsEnum() switch
        {
            MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Default => GetDefaultName(),
            MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => message.GetAzureServiceBus().QueueName(),
            MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => message.GetAzureServiceBus().TopicName(),
            _ => throw new ArgumentOutOfRangeException($"The Type {message.GetAzureServiceBus().Type().AsEnum()} is not supported.")
        };

        name = name.ToPascalCase();

        return prefix + name;

        string GetDefaultName()
        {
            var resolvedName = message.Name;
            resolvedName = resolvedName.RemoveSuffix("IntegrationEvent", "Event", "Message");
            resolvedName = resolvedName.ToPascalCase();
            return resolvedName;
        }
    }

    private static AzureServiceBusChannelType GetChannelType(MessageModel message)
    {
        if (string.IsNullOrWhiteSpace(message.GetAzureServiceBus()?.Type().Value))
        {
            return AzureServiceBusChannelType.Topic;
        }
        
        return message.GetAzureServiceBus().Type().AsEnum() switch
        {
            MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => AzureServiceBusChannelType.Topic,
            MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => AzureServiceBusChannelType.Queue,
            _ => AzureServiceBusChannelType.Topic
        };
    }

    public override string GetModelTypeName(IntentTemplateBase template)
    {
        return template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", MessageModel);
    }

    public override string GetSubscriberTypeName<T>(IntentTemplateBase<T> template)
    {
        return $"{template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventHandlerInterface")}<{GetModelTypeName(template)}>";
    }

    public override IEnumerable<IStereotype> Stereotypes => MessageModel.Stereotypes;

    public override string Name => MessageModel.Name;

    public override FolderModel Folder => MessageModel.Folder;

    public override IElement InternalElement => MessageModel.InternalElement;
}