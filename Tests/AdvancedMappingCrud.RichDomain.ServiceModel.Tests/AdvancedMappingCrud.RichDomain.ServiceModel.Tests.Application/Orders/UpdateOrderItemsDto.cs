using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Orders
{
    public class UpdateOrderItemsDto
    {
        public UpdateOrderItemsDto()
        {
            OrderItemDetails = null!;
        }

        public Guid Id { get; set; }
        public List<UpdateOrderItemsUpdateOrderItemUpdateDCDto> OrderItemDetails { get; set; }

        public static UpdateOrderItemsDto Create(
            Guid id,
            List<UpdateOrderItemsUpdateOrderItemUpdateDCDto> orderItemDetails)
        {
            return new UpdateOrderItemsDto
            {
                Id = id,
                OrderItemDetails = orderItemDetails
            };
        }
    }
}