using System;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Eventing.Messages
{
    public record OrderCreatedEvent : IEvent
    {
        public const string PubsubName = "pubsub";
        public const string TopicName = nameof(OrderCreatedEvent);
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        string IEvent.PubsubName => PubsubName;
        string IEvent.TopicName => TopicName;
    }
}