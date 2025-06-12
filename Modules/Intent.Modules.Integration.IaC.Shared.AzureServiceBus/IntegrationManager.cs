using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;

namespace Intent.Modules.Integration.IaC.Shared.AzureServiceBus;

internal class IntegrationManager
{
    private static IntegrationManager? _instance;
    public static void Initialize(ISoftwareFactoryExecutionContext executionContext)
    {
        _instance = new IntegrationManager(executionContext);
    }

    public static IntegrationManager Instance
    {
        get
        {
            if (_instance is null)
            {
                throw new InvalidOperationException("Azure Service Bus Manager not initialized.");
            }

            return _instance;
        }
    }
    
    private readonly List<MessageInfo> _publishedMessages;
    private readonly List<MessageInfo> _subscribedMessages; 
    private readonly List<CommandInfo> _sentCommands;
    private readonly List<CommandInfo> _receivedCommands;

    private IntegrationManager(ISoftwareFactoryExecutionContext executionContext)
    {
        var applications = executionContext.GetSolutionConfig()
            .GetApplicationReferences()
            .Select(app => executionContext.GetSolutionConfig().GetApplicationConfig(app.Id))
            .ToArray();

        const string azureServiceBusModule = "Intent.Eventing.AzureServiceBus";
        
        _publishedMessages = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == azureServiceBusModule))
            .SelectMany(app => executionContext.MetadataManager
                .GetExplicitlyPublishedMessageModels(app.Id)
                .Select(message => new MessageInfo(app.Id, message, null)))
            .Distinct()
            .ToList();
        _subscribedMessages = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == azureServiceBusModule))
            .SelectMany(app => executionContext.MetadataManager
                .Services(app.Id)
                .GetIntegrationEventHandlerModels()
                .SelectMany(handler => handler.IntegrationEventSubscriptions()
                    .Select(sub => new
                    {
                        Message = sub.Element.AsMessageModel(),
                        Handler = handler
                    }))
                .Select(sub => new MessageInfo(app.Id, sub.Message, sub.Handler)))
            .Distinct()
            .ToList();
        
        _sentCommands = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == azureServiceBusModule))
            .SelectMany(app => executionContext.MetadataManager
                .GetExplicitlySentIntegrationCommandModels(app.Id)
                .Select(command => new CommandInfo(app.Id, command, null)))
            .Distinct()
            .ToList();
        _receivedCommands = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == azureServiceBusModule))
            .SelectMany(app => executionContext.MetadataManager
                .Services(app.Id)
                .GetIntegrationEventHandlerModels()
                .SelectMany(handler => handler.IntegrationCommandSubscriptions()
                    .Select(sub => new
                    {
                        Command = sub.Element.AsIntegrationCommandModel(),
                        Handler = handler
                    }))
                .Select(sub => new CommandInfo(app.Id, sub.Command, sub.Handler)))
            .Distinct()
            .ToList();
    }

    public IReadOnlyList<AzureServiceBusMessage> GetPublishedAzureServiceBusMessages(string applicationId)
    {
        var messages = _publishedMessages
            .Where(p => p.ApplicationId == applicationId)
            .Select(s => new AzureServiceBusMessage(s.ApplicationId, s.Message, AzureServiceBusMethodType.Publish))
            .ToList();
        return messages;
    }
    
    public IReadOnlyList<AzureServiceBusMessage> GetSubscribedAzureServiceBusMessages(string applicationId)
    {
        var messages = _subscribedMessages
            .Where(p => p.ApplicationId == applicationId)
            .DistinctBy(s => s.Message.Id)
            .Select(s => new AzureServiceBusMessage(s.ApplicationId, s.Message, AzureServiceBusMethodType.Subscribe))
            .ToList();
        return messages;
    }
    
    public IReadOnlyList<AzureServiceBusCommand> GetPublishedAzureServiceBusCommands(string applicationId)
    {
        var messages = _sentCommands
            .Where(p => p.ApplicationId == applicationId)
            .Select(s => new AzureServiceBusCommand(s.ApplicationId, s.Command, AzureServiceBusMethodType.Publish))
            .ToList();
        return messages;
    }
    
    public IReadOnlyList<AzureServiceBusCommand> GetSubscribedAzureServiceBusCommands(string applicationId)
    {
        var messages = _receivedCommands
            .Where(p => p.ApplicationId == applicationId)
            .DistinctBy(s => s.Command.Id)
            .Select(s => new AzureServiceBusCommand(s.ApplicationId, s.Command, AzureServiceBusMethodType.Subscribe))
            .ToList();
        return messages;
    }

    public IReadOnlyList<AzureServiceBusItemBase> GetAggregatedPublishedAzureServiceBusItems(string applicationId)
    {
        return GetPublishedAzureServiceBusMessages(applicationId)
            .Cast<AzureServiceBusItemBase>()
            .Concat(GetPublishedAzureServiceBusCommands(applicationId))
            .ToList();
    }
    
    public IReadOnlyList<AzureServiceBusItemBase> GetAggregatedSubscribedAzureServiceBusItems(string applicationId)
    {
        return GetSubscribedAzureServiceBusMessages(applicationId)
            .Cast<AzureServiceBusItemBase>()
            .Concat(GetSubscribedAzureServiceBusCommands(applicationId))
            .ToList();
    }

    public IReadOnlyList<AzureServiceBusItemBase> GetAggregatedAzureServiceBusItems(string applicationId)
    {
        var duplicateCheckSet = new HashSet<string>();
        return GetAggregatedPublishedAzureServiceBusItems(applicationId)
            .Concat(GetAggregatedSubscribedAzureServiceBusItems(applicationId))
            .Where(p => duplicateCheckSet.Add(p.QueueOrTopicName))
            .ToList();
    }

    public IReadOnlyList<Subscription<AzureServiceBusItemBase>> GetAggregatedAzureServiceBusSubscriptions(string applicationId)
    {
        var results = new List<Subscription<AzureServiceBusItemBase>>();

        results.AddRange(_subscribedMessages
            .Where(message => message.ApplicationId == applicationId)
            .Select(message => new Subscription<AzureServiceBusItemBase>(
                message.EventHandlerModel!,
                new AzureServiceBusMessage(applicationId, message.Message, AzureServiceBusMethodType.Subscribe)))
        );

        results.AddRange(_receivedCommands
            .Where(command => command.ApplicationId == applicationId)
            .Select(command => new Subscription<AzureServiceBusItemBase>(
                command.EventHandlerModel!,
                new AzureServiceBusCommand(applicationId, command.Command, AzureServiceBusMethodType.Subscribe)))
        );

        return results;
    }

    public record Subscription<TSubscriptionItem>(IntegrationEventHandlerModel EventHandlerModel, TSubscriptionItem SubscriptionItem);
    
    private record MessageInfo(string ApplicationId, MessageModel Message, IntegrationEventHandlerModel? EventHandlerModel);
    private record CommandInfo(string ApplicationId, IntegrationCommandModel Command, IntegrationEventHandlerModel? EventHandlerModel);
}