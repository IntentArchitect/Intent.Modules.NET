using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;

namespace Intent.Modules.IaC.Terraform.Templates.Azure_Service_Bus;

internal class AzureServiceBusManager
{
    private static AzureServiceBusManager? _instance;
    public static void Initialize(IApplication application)
    {
        _instance = new AzureServiceBusManager(application);
    }

    public static AzureServiceBusManager Instance
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
    
    private AzureServiceBusManager(IApplication application)
    {
        _publishedMessages = application.GetSolutionConfig()
            .GetApplicationReferences()
            .SelectMany(app => application.MetadataManager
                .GetExplicitlyPublishedMessageModels(app.Id)
                .Select(m => new MessageInfo(app.Id, m)))
            .Distinct()
            .ToList();
    }

    public IReadOnlyList<Topic> GetTopics(string applicationId)
    {
        
    }

    public record Topic(string VariableName, string Name);
    private record MessageInfo(string ApplicationId, MessageModel Message);
}