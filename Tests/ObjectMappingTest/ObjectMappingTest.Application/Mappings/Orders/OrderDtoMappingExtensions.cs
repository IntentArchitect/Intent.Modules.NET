using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public static class OrderDtoMappingExtensions
    {
        public static OrderDto MapToOrderDto(this Order projectFrom)
        {
            return new OrderDto
            {
                Id = projectFrom.Id,
                RefNo = projectFrom.RefNo,
                CustomerId = projectFrom.CustomerId,
                OrderLines = projectFrom.Lines?.Select(x => x.MapToOrderLineDto()).ToList() ?? []
            };
        }

        public static List<OrderDto> MapToOrderDtoList(this IEnumerable<Order> projectFrom) => projectFrom.Select(x => x.MapToOrderDto()).ToList();
    }
}