using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TrainingModel.Tests.Application.Orders
{
    public class CreateOrderCommandOrderItemsDto
    {
        public CreateOrderCommandOrderItemsDto()
        {
        }

        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }

        public static CreateOrderCommandOrderItemsDto Create(decimal amount, int quantity, Guid productId)
        {
            return new CreateOrderCommandOrderItemsDto
            {
                Amount = amount,
                Quantity = quantity,
                ProductId = productId
            };
        }
    }
}