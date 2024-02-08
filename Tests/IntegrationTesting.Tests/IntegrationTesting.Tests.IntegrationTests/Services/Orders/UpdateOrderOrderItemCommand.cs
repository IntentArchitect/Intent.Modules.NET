using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Orders
{
    public class UpdateOrderOrderItemCommand
    {
        public UpdateOrderOrderItemCommand()
        {
            Description = null!;
        }

        public Guid OrderId { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid ProductId { get; set; }

        public static UpdateOrderOrderItemCommand Create(Guid orderId, Guid id, string description, Guid productId)
        {
            return new UpdateOrderOrderItemCommand
            {
                OrderId = orderId,
                Id = id,
                Description = description,
                ProductId = productId
            };
        }
    }
}