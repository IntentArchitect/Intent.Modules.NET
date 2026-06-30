using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public static class OrderLineDtoMappingExtensions
    {
        public static OrderLineDto MapToOrderLineDto(this OrderLine projectFrom)
        {
            return new OrderLineDto
            {
                Id = projectFrom.Id,
                ProductName = projectFrom.ProductName,
                Qty = projectFrom.Qty,
                DiscountCode = projectFrom.DiscountCode,
                UnitPrice = projectFrom.UnitPrice
            };
        }

        public static List<OrderLineDto> MapToOrderLineDtoList(this IEnumerable<OrderLine> projectFrom) => projectFrom.Select(x => x.MapToOrderLineDto()).ToList();
    }
}