using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;

namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal class IntegrationManager
{
    private static IntegrationManager? _instance;
    
    public static void Initialize(IApplication application)
    {
        _instance = new IntegrationManager(application);
    }

    public static IntegrationManager Instance
    {
        get
        {
            if (_instance is null)
            {
                throw new InvalidOperationException("AWS SQS Integration Manager not initialized.");
            }
            return _instance;
        }
    }
    
    private readonly List<MessageInfo> _publishedMessages;
    private readonly List<MessageInfo> _subscribedMessages;
    private readonly List<CommandInfo> _sentCommands;
    private readonly List<CommandInfo> _receivedCommands;

    private IntegrationManager(IApplication application)
    {
        var applications = application.GetSolutionConfig()
            .GetApplicationReferences()
            .Select(app => application.GetSolutionConfig().GetApplicationConfig(app.Id))
            .ToArray();

        const string awsSqsModule = "Intent.Aws.Sqs";
        
        // Collect published messages
        _publishedMessages = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == awsSqsModule))
            .SelectMany(app => application.MetadataManager
                .GetExplicitlyPublishedMessageModels(app.Id)
                .Select(message => new MessageInfo(app.Id, app.Name, message, null)))
            .Distinct()
            .ToList();
        
        // Collect subscribed messages
        _subscribedMessages = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == awsSqsModule))
            .SelectMany(app => application.MetadataManager
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
        
        // Collect sent commands
        _sentCommands = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == awsSqsModule))
            .SelectMany(app => application.MetadataManager
                .GetExplicitlySentIntegrationCommandModels(app.Id)
                .Select(command => new CommandInfo(app.Id, app.Name, command, null)))
            .Distinct()
            .ToList();
        
        // Collect received commands
        _receivedCommands = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == awsSqsModule))
            .SelectMany(app => application.MetadataManager
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

    public IReadOnlyList<SqsMessage> GetPublishedSqsMessages(string applicationId)
    {
        return _publishedMessages
            .Where(p => p.ApplicationId == applicationId)
            .Select(s => new SqsMessage(
                s.ApplicationId, 
                s.ApplicationName, 
                s.Message, 
                SqsMethodType.Publish))
            .ToList();
    }
    
    public IReadOnlyList<SqsMessage> GetSubscribedSqsMessages(string applicationId)
    {
        return _subscribedMessages
            .Where(p => p.ApplicationId == applicationId)
            .DistinctBy(s => s.Message.Id)
            .Select(s => new SqsMessage(
                s.ApplicationId, 
                s.ApplicationName, 
                s.Message, 
                SqsMethodType.Subscribe))
            .ToList();
    }

    public IReadOnlyList<SqsCommand> GetPublishedSqsCommands(string applicationId)
    {
        return _sentCommands
            .Where(p => p.ApplicationId == applicationId)
            .Select(s => new SqsCommand(
                s.ApplicationId, 
                s.ApplicationName, 
                s.Command, 
                SqsMethodType.Publish))
            .ToList();
    }

    public IReadOnlyList<SqsCommand> GetSubscribedSqsCommands(string applicationId)
    {
        return _receivedCommands
            .Where(p => p.ApplicationId == applicationId)
            .DistinctBy(s => s.Command.Id)
            .Select(s => new SqsCommand(
                s.ApplicationId, 
                s.ApplicationName, 
                s.Command, 
                SqsMethodType.Subscribe))
            .ToList();
    }

    public IReadOnlyList<SqsItemBase> GetAggregatedPublishedSqsItems(string applicationId)
    {
        var messages = GetPublishedSqsMessages(applicationId)
            .Cast<SqsItemBase>()
            .ToList();
        var commands = GetPublishedSqsCommands(applicationId)
            .Cast<SqsItemBase>()
            .ToList();
        return messages.Concat(commands).ToList();
    }
    
    public IReadOnlyList<SqsItemBase> GetAggregatedSubscribedSqsItems(string applicationId)
    {
        var messages = GetSubscribedSqsMessages(applicationId)
            .Cast<SqsItemBase>()
            .ToList();
        var commands = GetSubscribedSqsCommands(applicationId)
            .Cast<SqsItemBase>()
            .ToList();
        return messages.Concat(commands).ToList();
    }

    public IReadOnlyList<SqsItemBase> GetAggregatedSqsItems(string applicationId)
    {
        var duplicateCheckSet = new HashSet<string>();
        return GetAggregatedPublishedSqsItems(applicationId)
            .Concat(GetAggregatedSubscribedSqsItems(applicationId))
            .Where(p => duplicateCheckSet.Add($"{p.QueueName}.{p.MethodType}"))
            .ToList();
    }

    public IReadOnlyList<Subscription<SqsItemBase>> GetAggregatedSqsSubscriptions(string applicationId)
    {
        var messageSubscriptions = _subscribedMessages
            .Where(message => message.ApplicationId == applicationId)
            .Select(message => new Subscription<SqsItemBase>(
                message.EventHandlerModel!,
                new SqsMessage(
                    applicationId, 
                    message.ApplicationName, 
                    message.Message, 
                    SqsMethodType.Subscribe)))
            .ToList();

        var commandSubscriptions = _receivedCommands
            .Where(command => command.ApplicationId == applicationId)
            .Select(command => new Subscription<SqsItemBase>(
                command.EventHandlerModel!,
                new SqsCommand(
                    applicationId, 
                    command.ApplicationName, 
                    command.Command, 
                    SqsMethodType.Subscribe)))
            .ToList();

        return messageSubscriptions.Concat(commandSubscriptions).ToList();
    }

    public record Subscription<TSubscriptionItem>(
        IntegrationEventHandlerModel EventHandlerModel, 
        TSubscriptionItem SubscriptionItem);
    
    private record MessageInfo(
        string ApplicationId, 
        string ApplicationName, 
        MessageModel Message, 
        IntegrationEventHandlerModel? EventHandlerModel);
    
    private record CommandInfo(
        string ApplicationId, 
        string ApplicationName, 
        IntegrationCommandModel Command, 
        IntegrationEventHandlerModel? EventHandlerModel);
}
