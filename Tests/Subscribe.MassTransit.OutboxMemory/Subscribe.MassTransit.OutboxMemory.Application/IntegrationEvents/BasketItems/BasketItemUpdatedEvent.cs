using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public record BasketItemUpdatedEvent
    {
        public BasketItemUpdatedEvent()
        {
            Description = null!;
        }
        public Guid Id { get; init; }
        public string Description { get; init; }
        public decimal Amount { get; init; }
        public Guid BasketId { get; init; }
    }
}