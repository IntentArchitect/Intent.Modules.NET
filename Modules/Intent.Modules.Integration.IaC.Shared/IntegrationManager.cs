using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Eventing.AzureServiceBus.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Integration.IaC.Shared.AzureEventGrid;
using Intent.Modules.Integration.IaC.Shared.AzureServiceBus;

namespace Intent.Modules.Integration.IaC.Shared;

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
                throw new InvalidOperationException("Azure Service Bus Manager not initialized.");
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

        _publishedMessages = applications
            .SelectMany(app => application.MetadataManager
                .GetExplicitlyPublishedMessageModels(app.Id)
                .Select(message => new MessageInfo(app.Id, message, new ModuleInfo(app.Modules))))
            .Distinct()
            .ToList();
        _subscribedMessages = applications
            .SelectMany(app => application.MetadataManager
                .GetExplicitlySubscribedToMessageModels(app.Id)
                .Select(message => new MessageInfo(app.Id, message, new ModuleInfo(app.Modules))))
            .Distinct()
            .ToList();
        
        _sentCommands = applications
            .SelectMany(app => application.MetadataManager
                .GetExplicitlySentIntegrationCommandModels(app.Id)
                .Select(command => new CommandInfo(app.Id, command, new ModuleInfo(app.Modules))))
            .Distinct()
            .ToList();
        _receivedCommands = applications
            .SelectMany(app => application.MetadataManager
                .GetExplicitlySubscribedToIntegrationCommandModels(app.Id)
                .Select(command => new CommandInfo(app.Id, command, new ModuleInfo(app.Modules))))
            .Distinct()
            .ToList();
    }

    #region Azure Service Bus

    public IReadOnlyList<AzureServiceBusMessage> GetPublishedAzureServiceBusMessages(string applicationId)
    {
        var messages = _publishedMessages
            .Where(p => p.ModuleInfo.IsAzureServiceBus && p.ApplicationId == applicationId)
            .Select(s => new AzureServiceBusMessage(s.ApplicationId, s.Message, AzureServiceBusMethodType.Publish))
            .ToList();
        return messages;
    }
    
    public IReadOnlyList<AzureServiceBusMessage> GetSubscribedAzureServiceBusMessages(string applicationId)
    {
        var messages = _subscribedMessages
            .Where(p => p.ModuleInfo.IsAzureServiceBus && p.ApplicationId == applicationId)
            .Select(s => new AzureServiceBusMessage(s.ApplicationId, s.Message, AzureServiceBusMethodType.Subscribe))
            .ToList();
        return messages;
    }
    
    public IReadOnlyList<AzureServiceBusCommand> GetPublishedAzureServiceBusCommands(string applicationId)
    {
        var messages = _sentCommands
            .Where(p => p.ModuleInfo.IsAzureServiceBus && p.ApplicationId == applicationId)
            .Select(s => new AzureServiceBusCommand(s.ApplicationId, s.Command, AzureServiceBusMethodType.Publish))
            .ToList();
        return messages;
    }
    
    public IReadOnlyList<AzureServiceBusCommand> GetSubscribedAzureServiceBusCommands(string applicationId)
    {
        var messages = _receivedCommands
            .Where(p => p.ModuleInfo.IsAzureServiceBus && p.ApplicationId == applicationId)
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

    #endregion

    #region Azure Event Grid

    public IReadOnlyList<AzureEventGridMessage> GetPublishedAzureEventGridMessages(string applicationId)
    {
        var messages = _publishedMessages
            .Where(p => p.ModuleInfo.IsAzureEventGrid && p.ApplicationId == applicationId)
            .Select(s => new AzureEventGridMessage(s.ApplicationId, s.Message, AzureEventGridMethodType.Publish))
            .ToList();
        return messages;
    }
    
    public IReadOnlyList<AzureEventGridMessage> GetSubscribedAzureEventGridMessages(string applicationId)
    {
        var messages = _subscribedMessages
            .Where(p => p.ModuleInfo.IsAzureEventGrid && p.ApplicationId == applicationId)
            .Select(s => new AzureEventGridMessage(s.ApplicationId, s.Message, AzureEventGridMethodType.Subscribe))
            .ToList();
        return messages;
    }

    public IReadOnlyList<AzureEventGridMessage> GetAggregatedAzureEventGridMessages(string applicationId)
    {
        return GetPublishedAzureEventGridMessages(applicationId)
            .Concat(GetSubscribedAzureEventGridMessages(applicationId))
            .ToList();
    }

    #endregion

    private record MessageInfo(string ApplicationId, MessageModel Message, ModuleInfo ModuleInfo);
    private record CommandInfo(string ApplicationId, IntegrationCommandModel Command, ModuleInfo ModuleInfo);

    private record ModuleInfo
    {
        public ModuleInfo(ICollection<IIntentInstalledModule> modules)
        {
            IsAzureEventGrid = modules.Any(m => m.ModuleId == "Intent.Eventing.AzureEventGrid");
            IsAzureServiceBus = modules.Any(m => m.ModuleId == "Intent.Eventing.AzureServiceBus");
        }
        
        public bool IsAzureEventGrid { get; }
        public bool IsAzureServiceBus { get; }
    }
}