using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class BasketUpdatedEventExtensions
    {
        public static BasketUpdatedEvent MapToBasketUpdatedEvent(this Basket projectFrom)
        {
            return new BasketUpdatedEvent
            {
                Id = projectFrom.Id,
                Number = projectFrom.Number,
                BasketItems = projectFrom.BasketItems.Select(BasketItemDtoExtensions.MapToBasketItemDto).ToList(),
            };
        }
    }
}