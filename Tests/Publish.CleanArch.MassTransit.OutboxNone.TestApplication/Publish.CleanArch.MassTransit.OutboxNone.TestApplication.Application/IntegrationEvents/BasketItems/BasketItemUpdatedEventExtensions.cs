using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class BasketItemUpdatedEventExtensions
    {
        public static BasketItemUpdatedEvent MapToBasketItemUpdatedEvent(this BasketItem projectFrom)
        {
            return new BasketItemUpdatedEvent
            {
                Id = projectFrom.Id,
                Description = projectFrom.Description,
                Amount = projectFrom.Amount,
                BasketId = projectFrom.BasketId,
            };
        }
    }
}