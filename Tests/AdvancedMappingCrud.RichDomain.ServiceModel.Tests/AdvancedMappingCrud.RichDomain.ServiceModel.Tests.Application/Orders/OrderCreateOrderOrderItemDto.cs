using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Orders
{
    public class OrderCreateOrderOrderItemDto
    {
        public OrderCreateOrderOrderItemDto()
        {
        }

        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }

        public static OrderCreateOrderOrderItemDto Create(Guid productId, int quantity, decimal amount)
        {
            return new OrderCreateOrderOrderItemDto
            {
                ProductId = productId,
                Quantity = quantity,
                Amount = amount
            };
        }
    }
}