using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Orders
{
    public class CreateOrderOrderItemDto
    {
        public CreateOrderOrderItemDto()
        {
        }

        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }

        public static CreateOrderOrderItemDto Create(Guid productId, int quantity, decimal amount)
        {
            return new CreateOrderOrderItemDto
            {
                ProductId = productId,
                Quantity = quantity,
                Amount = amount
            };
        }
    }
}