using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public class OrderItemDto
    {
        public OrderItemDto()
        {
        }

        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public static OrderItemDto Create(Guid id, Guid orderId, string description, decimal amount)
        {
            return new OrderItemDto
            {
                Id = id,
                OrderId = orderId,
                Description = description,
                Amount = amount
            };
        }
    }
}