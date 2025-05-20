using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Eventing.AzureServiceBus.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;

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
    }

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

    private record MessageInfo(string ApplicationId, MessageModel Message, ModuleInfo ModuleInfo);

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