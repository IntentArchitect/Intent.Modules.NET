using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Orders
{
    public class UpdateOrderItemsUpdateOrderItemUpdateDCDto
    {
        public UpdateOrderItemsUpdateOrderItemUpdateDCDto()
        {
        }

        public Guid? Id { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }

        public static UpdateOrderItemsUpdateOrderItemUpdateDCDto Create(Guid? id, decimal amount, int quantity, Guid productId)
        {
            return new UpdateOrderItemsUpdateOrderItemUpdateDCDto
            {
                Id = id,
                Amount = amount,
                Quantity = quantity,
                ProductId = productId
            };
        }
    }
}