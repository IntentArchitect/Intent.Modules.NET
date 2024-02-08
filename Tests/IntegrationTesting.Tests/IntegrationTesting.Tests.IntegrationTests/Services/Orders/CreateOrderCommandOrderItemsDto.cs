using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Orders
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