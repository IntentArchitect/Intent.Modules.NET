using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public class BasketItemDto
    {
        public BasketItemDto()
        {
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public Guid BasketId { get; set; }

        public static BasketItemDto Create(Guid id, string description, decimal amount, Guid basketId)
        {
            return new BasketItemDto
            {
                Id = id,
                Description = description,
                Amount = amount,
                BasketId = basketId
            };
        }
    }
}