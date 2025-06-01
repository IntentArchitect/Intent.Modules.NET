using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders
{
    public class OrderCreateDto
    {
        public OrderCreateDto()
        {
            RefNo = null!;
            OrderItems = null!;
        }

        public Guid CustomerId { get; set; }
        public string RefNo { get; set; }
        public List<CreateOrderOrderItemsDto> OrderItems { get; set; }

        public static OrderCreateDto Create(Guid customerId, string refNo, List<CreateOrderOrderItemsDto> orderItems)
        {
            return new OrderCreateDto
            {
                CustomerId = customerId,
                RefNo = refNo,
                OrderItems = orderItems
            };
        }
    }
}