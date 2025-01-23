using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared.Orders
{
    public record OrderDeletedEvent
    {
        public OrderDeletedEvent()
        {
            Number = null!;
        }

        public Guid Id { get; init; }
        public string Number { get; init; }
    }
}