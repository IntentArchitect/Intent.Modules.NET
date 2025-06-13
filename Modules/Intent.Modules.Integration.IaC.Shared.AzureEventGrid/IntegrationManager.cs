using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;

namespace Intent.Modules.Integration.IaC.Shared.AzureEventGrid;

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

        const string azureEventGridModule = "Intent.Eventing.AzureEventGrid";
        
        _publishedMessages = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == azureEventGridModule))
            .SelectMany(app => executionContext.MetadataManager
                .GetExplicitlyPublishedMessageModels(app.Id)
                .Select(message => new MessageInfo(app.Id, app.Name, message, null)))
            .Distinct()
            .ToList();
        _subscribedMessages = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == azureEventGridModule))
            .SelectMany(app => executionContext.MetadataManager
                .Services(app.Id)
                .GetIntegrationEventHandlerModels()
                .SelectMany(handler => handler.IntegrationEventSubscriptions()
                    .Select(sub => new
                    {
                        Message = sub.Element.AsMessageModel(),
                        Handler = handler
                    }))
                .Select(sub => new MessageInfo(app.Id, app.Name, sub.Message, sub.Handler)))
            .Distinct()
            .ToList();
        
        _sentCommands = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == azureEventGridModule))
            .SelectMany(app => executionContext.MetadataManager
                .GetExplicitlySentIntegrationCommandModels(app.Id)
                .Select(command => new CommandInfo(app.Id, app.Name, command, null)))
            .Distinct()
            .ToList();
        _receivedCommands = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == azureEventGridModule))
            .SelectMany(app => executionContext.MetadataManager
                .Services(app.Id)
                .GetIntegrationEventHandlerModels()
                .SelectMany(handler => handler.IntegrationCommandSubscriptions()
                    .Select(sub => new
                    {
                        Command = sub.Element.AsIntegrationCommandModel(),
                        Handler = handler
                    }))
                .Select(sub => new CommandInfo(app.Id, app.Name, sub.Command, sub.Handler)))
            .Distinct()
            .ToList();
    }

    public IReadOnlyList<AzureEventGridMessage> GetPublishedAzureEventGridMessages(string applicationId)
    {
        var messages = _publishedMessages
            .Where(p => p.ApplicationId == applicationId)
            .Select(s => new AzureEventGridMessage(s.ApplicationId, s.ApplicationName, s.Message, AzureEventGridMethodType.Publish))
            .ToList();
        return messages;
    }
    
    public IReadOnlyList<AzureEventGridMessage> GetSubscribedAzureEventGridMessages(string applicationId)
    {
        var messages = _subscribedMessages
            .Where(p => p.ApplicationId == applicationId)
            .Select(s => new AzureEventGridMessage(s.ApplicationId, s.ApplicationName, s.Message, AzureEventGridMethodType.Subscribe))
            .ToList();
        return messages;
    }

    public IReadOnlyList<AzureEventGridMessage> GetAggregatedAzureEventGridMessages(string applicationId)
    {
        return GetPublishedAzureEventGridMessages(applicationId)
            .Concat(GetSubscribedAzureEventGridMessages(applicationId))
            .ToList();
    }
    
    public IReadOnlyList<Subscription<AzureEventGridMessage>> GetAggregatedAzureEventGridSubscriptions(string applicationId)
    {
        var results = new List<Subscription<AzureEventGridMessage>>();

        results.AddRange(_subscribedMessages
            .Where(message => message.ApplicationId == applicationId)
            .Select(message => new Subscription<AzureEventGridMessage>(
                message.EventHandlerModel!,
                new AzureEventGridMessage(applicationId, message.ApplicationName, message.Message, AzureEventGridMethodType.Subscribe)))
        );

        return results;
    }

    public record Subscription<TSubscriptionItem>(IntegrationEventHandlerModel EventHandlerModel, TSubscriptionItem SubscriptionItem);
    
    private record MessageInfo(string ApplicationId, string ApplicationName, MessageModel Message, IntegrationEventHandlerModel? EventHandlerModel);
    private record CommandInfo(string ApplicationId, string ApplicationName, IntegrationCommandModel Command, IntegrationEventHandlerModel? EventHandlerModel);
}