using System;
using System.Collections.Generic;
using Google.Cloud.PubSub.V1;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.GoogleEventBusTopicEventManager", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Eventing;

public class GoogleEventBusTopicEventManager : IEventBusTopicEventManager
{
    private readonly ICloudResourceManager _resourceManager;
    private readonly Dictionary<string, string> _topicLookup;

    public GoogleEventBusTopicEventManager(ICloudResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
        _topicLookup = new Dictionary<string, string>();
    }

    public void RegisterTopicEvent<TMessage>(string topicId) where TMessage : class
    {
        _topicLookup.Add(typeof(TMessage).FullName!, topicId);
    }

    public TopicName GetTopicName(PubsubMessage message)
    {
        var messageType = message.Attributes["MessageType"]!;
        if (!_topicLookup.TryGetValue(messageType, out var topicId))
        {
            throw new InvalidOperationException($"Could not find a Topic Id for Message Type: {messageType}");
        }
        return new TopicName(_resourceManager.ProjectId, topicId);
    }
}