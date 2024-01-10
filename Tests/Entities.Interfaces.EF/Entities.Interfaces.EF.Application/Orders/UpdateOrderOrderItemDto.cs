using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Orders
{
    public class UpdateOrderOrderItemDto
    {
        public UpdateOrderOrderItemDto()
        {
            Description = null!;
        }

        public string Description { get; set; }
        public Guid OrderId { get; set; }
        public Guid Id { get; set; }

        public static UpdateOrderOrderItemDto Create(string description, Guid orderId, Guid id)
        {
            return new UpdateOrderOrderItemDto
            {
                Description = description,
                OrderId = orderId,
                Id = id
            };
        }
    }
}