using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Orders
{
    public class CreateOrderCommandOrderItemsDto
    {
        public CreateOrderCommandOrderItemsDto()
        {
            Description = null!;
        }

        public string Description { get; set; }
        public Guid ProductId { get; set; }

        public static CreateOrderCommandOrderItemsDto Create(string description, Guid productId)
        {
            return new CreateOrderCommandOrderItemsDto
            {
                Description = description,
                ProductId = productId
            };
        }
    }
}