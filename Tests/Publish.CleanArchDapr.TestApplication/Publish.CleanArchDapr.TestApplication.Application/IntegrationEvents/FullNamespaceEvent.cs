using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Eventing.Messages
{
    public record FullNamespaceEvent : IEvent
    {
        public const string PubsubName = "pubsub";
        public const string TopicName = "Publish.CleanArchDapr.TestApplication.Eventing.Messages.FullNamespaceEvent";
        string IEvent.PubsubName => PubsubName;
        string IEvent.TopicName => TopicName;
    }
}