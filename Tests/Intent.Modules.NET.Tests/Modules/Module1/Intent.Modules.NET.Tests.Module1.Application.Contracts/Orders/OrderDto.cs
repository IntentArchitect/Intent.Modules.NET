using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders
{
    public class OrderDto
    {
        public OrderDto()
        {
            RefNo = null!;
            OrderItems = null!;
        }

        public Guid CustomerId { get; set; }
        public string RefNo { get; set; }
        public Guid Id { get; set; }
        public List<OrderOrderItemDto> OrderItems { get; set; }

        public static OrderDto Create(Guid customerId, string refNo, Guid id, List<OrderOrderItemDto> orderItems)
        {
            return new OrderDto
            {
                CustomerId = customerId,
                RefNo = refNo,
                Id = id,
                OrderItems = orderItems
            };
        }
    }
}