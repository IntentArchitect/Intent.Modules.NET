using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Orders
{
    public class CreateOrderOrderItemCommand
    {
        public CreateOrderOrderItemCommand()
        {
            Description = null!;
        }

        public Guid OrderId { get; set; }
        public string Description { get; set; }
        public Guid ProductId { get; set; }

        public static CreateOrderOrderItemCommand Create(Guid orderId, string description, Guid productId)
        {
            return new CreateOrderOrderItemCommand
            {
                OrderId = orderId,
                Description = description,
                ProductId = productId
            };
        }
    }
}