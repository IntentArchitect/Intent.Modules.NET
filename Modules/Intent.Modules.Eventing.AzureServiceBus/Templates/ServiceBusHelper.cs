using System;
using System.Threading;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.AzureServiceBus.Api;

namespace Intent.Modules.Eventing.AzureServiceBus.Templates;

public static class ServiceBusHelper
{
    public static string GetCommandQueueOrTopicName(this IntegrationCommandModel command)
    {
        if (command.HasAzureServiceBus())
        {
            var name = command.GetAzureServiceBus().Type().AsEnum() switch
            {
                IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => command.GetAzureServiceBus().QueueName(),
                IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => command.GetAzureServiceBus().TopicName(),
                _ => throw new ArgumentOutOfRangeException($"The Type {command.GetAzureServiceBus().Type().AsEnum()} is not supported.")
            };

            return name;
        }
        
        var resolvedName = command.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationCommand", "Command", "Message");
        resolvedName = resolvedName.ToKebabCase();
        return resolvedName;
    }
    
    public static string GetCommandQueueOrTopicConfigurationName(this IntegrationCommandModel command)
    {
        if (command.HasAzureServiceBus())
        {
            var name = command.GetAzureServiceBus().Type().AsEnum() switch
            {
                IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => command.GetAzureServiceBus().QueueName(),
                IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => command.GetAzureServiceBus().TopicName(),
                _ => throw new ArgumentOutOfRangeException($"The Type {command.GetAzureServiceBus().Type().AsEnum()} is not supported.")
            };

            name = name.ToPascalCase();

            return name;
        }

        var resolvedName = command.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationCommand", "Command", "Message");
        resolvedName = resolvedName.ToPascalCase();
        return resolvedName;
    }
    
    public static string GetMessageQueueOrTopicName(this MessageModel command)
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
    
    public static string GetMessageQueueOrTopicConfigurationName(this MessageModel message)
    {
        if (message.HasAzureServiceBus())
        {
            var name = message.GetAzureServiceBus().Type().AsEnum() switch
            {
                MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => message.GetAzureServiceBus().QueueName(),
                MessageModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => message.GetAzureServiceBus().TopicName(),
                _ => throw new ArgumentOutOfRangeException($"The Type {message.GetAzureServiceBus().Type().AsEnum()} is not supported.")
            };

            name = name.ToPascalCase();

            return name;
        }

        var resolvedName = message.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationEvent", "Event", "Message");
        resolvedName = resolvedName.ToPascalCase();
        return resolvedName;
    }

    public static bool HasCommandSubscription(this IntegrationCommandModel command)
    {
        return command.GetAzureServiceBus()?.Type().IsTopic() == true;
    }

    public static string? GetCommandSubscriptionConfigurationName(this IntegrationCommandModel command)
    {
        return command.HasCommandSubscription() ? command.GetCommandQueueOrTopicConfigurationName() + "Subscription" : null;
    }

    public static bool HasMessageSubscription(this MessageModel message)
    {
        return message.GetAzureServiceBus()?.Type().IsTopic() != false;
    }

    public static string? GetMessageSubscriptionConfigurationName(this MessageModel message)
    {
        return message.HasMessageSubscription() ? message.GetMessageQueueOrTopicConfigurationName() + "Subscription" : null;
    }
}