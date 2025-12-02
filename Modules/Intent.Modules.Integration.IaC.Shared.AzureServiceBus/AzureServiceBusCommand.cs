using System;
using System.Collections.Generic;
using Intent.Eventing.AzureServiceBus.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Integration.IaC.Shared.AzureServiceBus;

internal record AzureServiceBusCommand : AzureServiceBusItemBase
{
    public AzureServiceBusCommand(string ApplicationId, string ApplicationName, IntegrationCommandModel CommandModel, AzureServiceBusMethodType MethodType)
    {
        this.ApplicationId = ApplicationId;
        this.ApplicationName = ApplicationName;
        this.CommandModel = CommandModel;
        this.MethodType = MethodType;
        ChannelType = GetChannelType(CommandModel);
        QueueOrTopicName = GetQueueOrTopicName(CommandModel);
        QueueOrTopicConfigurationName = GetQueueOrTopicConfigurationName(CommandModel);
        QueueOrTopicSubscriptionConfigurationName = MethodType == AzureServiceBusMethodType.Subscribe && ChannelType == AzureServiceBusChannelType.Topic 
            ? $"{QueueOrTopicConfigurationName}Subscription".ToPascalCase() : null;
    }
    
    public IntegrationCommandModel CommandModel { get; init; }

    private static string GetQueueOrTopicName(IntegrationCommandModel command)
    {
        if (!command.HasAzureServiceBus())
        {
            return GetDefaultName();
        }
        
        var name = command.GetAzureServiceBus().Type().AsEnum() switch
        {
            IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Default => GetDefaultName(),
            IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => command.GetAzureServiceBus().QueueName(),
            IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => command.GetAzureServiceBus().TopicName(),
            _ => throw new ArgumentOutOfRangeException($"The Type {command.GetAzureServiceBus().Type().AsEnum()} is not supported.")
        };

        return name;

        string GetDefaultName()
        {
            var resolvedName = command.Name;
            resolvedName = resolvedName.RemoveSuffix("IntegrationCommand", "Event", "Message");
            resolvedName = resolvedName.ToKebabCase();
            return resolvedName;
        }
    }

    private static string GetQueueOrTopicConfigurationName(IntegrationCommandModel command)
    {
        const string prefix = "AzureServiceBus:";
        if (!command.HasAzureServiceBus())
        {
            return prefix + GetDefaultName();
        }
        
        var name = command.GetAzureServiceBus().Type().AsEnum() switch
        {
            IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Default => GetDefaultName(),
            IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => command.GetAzureServiceBus().QueueName(),
            IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => command.GetAzureServiceBus().TopicName(),
            _ => throw new ArgumentOutOfRangeException($"The Type {command.GetAzureServiceBus().Type().AsEnum()} is not supported.")
        };

        name = name.ToPascalCase();

        return prefix + name;

        string GetDefaultName()
        {
            var resolvedName = command.Name;
            resolvedName = resolvedName.RemoveSuffix("IntegrationCommand", "Event", "Message");
            resolvedName = resolvedName.ToPascalCase();
            return resolvedName;
        }
    }

    private static AzureServiceBusChannelType GetChannelType(IntegrationCommandModel message)
    {
        if (string.IsNullOrWhiteSpace(message.GetAzureServiceBus()?.Type().Value))
        {
            return AzureServiceBusChannelType.Queue;
        }
        
        return message.GetAzureServiceBus().Type().AsEnum() switch
        {
            IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Queue => AzureServiceBusChannelType.Queue,
            IntegrationCommandModelStereotypeExtensions.AzureServiceBus.TypeOptionsEnum.Topic => AzureServiceBusChannelType.Topic,
            _ => AzureServiceBusChannelType.Queue
        };
    }

    public override string GetModelTypeName(IntentTemplateBase template)
    {
        return template.GetTypeName("Intent.Eventing.Contracts.IntegrationCommand", CommandModel);
    }

    public override string GetSubscriberTypeName<T>(IntentTemplateBase<T> template)
    {
        return $"{template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventHandlerInterface")}<{GetModelTypeName(template)}>";
    }

    public override IEnumerable<IStereotype> Stereotypes => CommandModel.Stereotypes;

    public override string Name => CommandModel.Name;

    public override FolderModel Folder => CommandModel.Folder;

    public override IElement InternalElement => CommandModel.InternalElement;
}