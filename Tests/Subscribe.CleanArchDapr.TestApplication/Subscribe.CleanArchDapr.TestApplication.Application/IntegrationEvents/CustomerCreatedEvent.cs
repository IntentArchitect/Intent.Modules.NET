using Intent.RoslynWeaver.Attributes;
using Subscribe.CleanArchDapr.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Eventing.Messages
{
    public record CustomerCreatedEvent : IEvent
    {
        public const string PubsubName = "pubsub";
        public const string TopicName = nameof(CustomerCreatedEvent);

        public CustomerCreatedEvent()
        {
            Name = null!;
        }

        public string Name { get; init; }
        string IEvent.PubsubName => PubsubName;
        string IEvent.TopicName => TopicName;
    }
}