using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public record BasketCreatedEvent
    {
        public BasketCreatedEvent()
        {
            Number = null!;
            BasketItems = null!;
        }
        public Guid Id { get; init; }
        public string Number { get; init; }
        public List<BasketItemDto> BasketItems { get; init; }
    }
}