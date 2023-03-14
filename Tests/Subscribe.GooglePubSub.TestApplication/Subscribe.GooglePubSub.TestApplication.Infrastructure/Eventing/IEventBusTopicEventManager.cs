using System;
using Google.Cloud.PubSub.V1;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.InterfaceTemplates.EventBusTopicEventManagerInterface", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Infrastructure.Eventing;

public interface IEventBusTopicEventManager
{
    void RegisterTopicEvent<TMessage>(string topicId) where TMessage : class;
    TopicName GetTopicName(PubsubMessage message);
}