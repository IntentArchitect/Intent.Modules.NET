using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public static class OrderSummaryDtoMappingExtensions
    {
        public static OrderSummaryDto MapToOrderSummaryDto(this Order projectFrom)
        {
            return new OrderSummaryDto
            {
                Id = projectFrom.Id,
                RefNo = projectFrom.RefNo,
                PaymentStatus = (PaymentStatusDto)projectFrom.PaymentStatus,
                DisplayName = projectFrom.GetDisplayName()
            };
        }

        public static List<OrderSummaryDto> MapToOrderSummaryDtoList(this IEnumerable<Order> projectFrom) => projectFrom.Select(x => x.MapToOrderSummaryDto()).ToList();
    }
}