using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class BasketCreatedEventExtensions
    {
        public static BasketCreatedEvent MapToBasketCreatedEvent(this Basket projectFrom)
        {
            return new BasketCreatedEvent
            {
                Id = projectFrom.Id,
                Number = projectFrom.Number,
                BasketItems = projectFrom.BasketItems.Select(BasketItemDtoExtensions.MapToBasketItemDto).ToList(),
            };
        }
    }
}