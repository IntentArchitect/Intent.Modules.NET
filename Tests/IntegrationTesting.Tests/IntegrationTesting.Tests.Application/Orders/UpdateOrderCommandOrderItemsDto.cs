using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Orders
{
    public class UpdateOrderCommandOrderItemsDto
    {
        public UpdateOrderCommandOrderItemsDto()
        {
            Description = null!;
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid ProductId { get; set; }

        public static UpdateOrderCommandOrderItemsDto Create(Guid id, string description, Guid productId)
        {
            return new UpdateOrderCommandOrderItemsDto
            {
                Id = id,
                Description = description,
                ProductId = productId
            };
        }
    }
}