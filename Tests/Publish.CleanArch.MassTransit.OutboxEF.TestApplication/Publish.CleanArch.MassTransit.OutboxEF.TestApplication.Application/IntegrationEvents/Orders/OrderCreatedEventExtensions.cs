using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class OrderCreatedEventExtensions
    {
        public static OrderCreatedEvent MapToOrderCreatedEvent(this Order projectFrom)
        {
            return new OrderCreatedEvent
            {
                Id = projectFrom.Id,
                Number = projectFrom.Number,
                OrderItems = projectFrom.OrderItems.Select(OrderItemDtoExtensions.MapToOrderItemDto).ToList(),
            };
        }
    }
}