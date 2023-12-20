using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders
{
    public class CreateOrderCommandOrderItemsDto
    {
        public CreateOrderCommandOrderItemsDto()
        {
        }

        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public Guid ProductId { get; set; }

        public static CreateOrderCommandOrderItemsDto Create(int quantity, decimal amount, Guid productId)
        {
            return new CreateOrderCommandOrderItemsDto
            {
                Quantity = quantity,
                Amount = amount,
                ProductId = productId
            };
        }
    }
}