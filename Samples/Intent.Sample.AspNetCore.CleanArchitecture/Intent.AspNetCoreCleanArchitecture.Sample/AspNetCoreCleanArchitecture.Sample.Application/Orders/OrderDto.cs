using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Orders
{
    public class OrderDto
    {
        public OrderDto()
        {
            OrderNo = null!;
            OrderLines = null!;
        }

        public Guid Id { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderOrderOrderLineDto> OrderLines { get; set; }

        public static OrderDto Create(Guid id, string orderNo, DateTime orderDate, List<OrderOrderOrderLineDto> orderLines)
        {
            return new OrderDto
            {
                Id = id,
                OrderNo = orderNo,
                OrderDate = orderDate,
                OrderLines = orderLines
            };
        }
    }
}