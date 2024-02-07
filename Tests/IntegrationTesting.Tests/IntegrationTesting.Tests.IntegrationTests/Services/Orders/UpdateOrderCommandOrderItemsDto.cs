using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Orders
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