using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Contracts
{
    public record OrderItemUpdateDC
    {
        public OrderItemUpdateDC(Guid? id, decimal amount, int quantity, Guid productId)
        {
            Id = id;
            Amount = amount;
            Quantity = quantity;
            ProductId = productId;
        }

        public Guid? Id { get; init; }
        public decimal Amount { get; init; }
        public int Quantity { get; init; }
        public Guid ProductId { get; init; }
    }
}