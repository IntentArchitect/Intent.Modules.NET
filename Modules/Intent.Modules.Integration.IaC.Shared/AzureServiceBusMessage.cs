using System;
using Intent.Eventing.AzureServiceBus.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Integration.IaC.Shared;

internal enum AzureServiceBusChannelType
{
    Queue,
    Topic
}

internal enum AzureServiceBusMethodType
{
    Publish = 1,
    Subscribe = 2
}

internal record AzureServiceBusMessage
{
    public AzureServiceBusMessage(string ApplicationId, MessageModel MessageModel, AzureServiceBusMethodType MethodType)
    {
        this.ApplicationId = ApplicationId;
        this.MessageModel = MessageModel;
        this.MethodType = MethodType;
        ChannelType = GetChannelType(MessageModel);
        QueueOrTopicName = GetQueueOrTopicName(MessageModel);
        QueueOrTopicConfigurationName = GetQueueOrTopicConfigurationName(MessageModel);
        QueueOrTopicSubscriptionConfigurationName = MethodType == AzureServiceBusMethodType.Subscribe && ChannelType == AzureServiceBusChannelType.Topic 
            ? $"{QueueOrTopicConfigurationName}Subscription".ToPascalCase() : null;
    }

    public string ApplicationId { get; init; }
    public MessageModel MessageModel { get; init; }
    public AzureServiceBusMethodType MethodType { get; init; }
    public AzureServiceBusChannelType ChannelType { get; init; }
    public string QueueOrTopicName { get; init; }
    public string QueueOrTopicConfigurationName { get; init; }
    public string? QueueOrTopicSubscriptionConfigurationName { get; init; }

    private static string GetQueueOrTopicName(MessageModel command)
    {
        if (command.HasAzureServiceBus())
        {
            var name = command.GetAzureServiceBus().Type().AsEnum() switch
            {
                MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => command.GetAzureServiceBus().QueueName(),
                MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => command.GetAzureServiceBus().TopicName(),
                _ => throw new ArgumentOutOfRangeException($"The Type {command.GetAzureServiceBus().Type().AsEnum()} is not supported.")
            };

            return name;
        }
        
        var resolvedName = command.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationEvent", "Event", "Message");
        resolvedName = resolvedName.ToKebabCase();
        return resolvedName;
    }

    private static string GetQueueOrTopicConfigurationName(MessageModel message)
    {
        const string prefix = "AzureServiceBus:";
        if (message.HasAzureServiceBus())
        {
            var name = message.GetAzureServiceBus().Type().AsEnum() switch
            {
                MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => message.GetAzureServiceBus().QueueName(),
                MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => message.GetAzureServiceBus().TopicName(),
                _ => throw new ArgumentOutOfRangeException($"The Type {message.GetAzureServiceBus().Type().AsEnum()} is not supported.")
            };

            name = name.ToPascalCase();

            return prefix + name;
        }

        var resolvedName = message.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationEvent", "Event", "Message");
        resolvedName = resolvedName.ToPascalCase();
        return prefix + resolvedName;
    }

    private static AzureServiceBusChannelType GetChannelType(MessageModel message)
    {
        return message.GetAzureServiceBus()?.Type().IsTopic() switch
        {
            true => AzureServiceBusChannelType.Topic,
            false => AzureServiceBusChannelType.Queue,
            _ => AzureServiceBusChannelType.Topic
        };
    }
}