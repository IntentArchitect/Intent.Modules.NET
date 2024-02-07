using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Orders
{
    public class OrderDto
    {
        public OrderDto()
        {
            RefNo = null!;
            OrderItems = null!;
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string RefNo { get; set; }
        public List<OrderOrderItemDto> OrderItems { get; set; }

        public static OrderDto Create(Guid id, Guid customerId, string refNo, List<OrderOrderItemDto> orderItems)
        {
            return new OrderDto
            {
                Id = id,
                CustomerId = customerId,
                RefNo = refNo,
                OrderItems = orderItems
            };
        }
    }
}