using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Orders
{
    public class CreateOrderCommand
    {
        public CreateOrderCommand()
        {
            RefNo = null!;
            OrderItems = null!;
        }

        public Guid CustomerId { get; set; }
        public string RefNo { get; set; }
        public List<CreateOrderCommandOrderItemsDto> OrderItems { get; set; }

        public static CreateOrderCommand Create(
            Guid customerId,
            string refNo,
            List<CreateOrderCommandOrderItemsDto> orderItems)
        {
            return new CreateOrderCommand
            {
                CustomerId = customerId,
                RefNo = refNo,
                OrderItems = orderItems
            };
        }
    }
}