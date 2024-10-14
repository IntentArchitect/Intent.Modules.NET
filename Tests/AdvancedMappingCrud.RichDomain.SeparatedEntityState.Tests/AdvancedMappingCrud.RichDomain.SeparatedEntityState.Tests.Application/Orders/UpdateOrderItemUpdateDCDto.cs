using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Orders
{
    public class UpdateOrderItemUpdateDCDto
    {
        public UpdateOrderItemUpdateDCDto()
        {
        }

        public Guid? Id { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }

        public static UpdateOrderItemUpdateDCDto Create(Guid? id, decimal amount, int quantity, Guid productId)
        {
            return new UpdateOrderItemUpdateDCDto
            {
                Id = id,
                Amount = amount,
                Quantity = quantity,
                ProductId = productId
            };
        }
    }
}