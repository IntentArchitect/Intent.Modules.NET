using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Strict.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMapping.Strict.Application.Orders
{
    public static class OrderDtoMappingExtensions
    {
        public static OrderDto MapToOrderDto(this Order projectFrom)
        {
            return new OrderDto
            {
                OrderNumber = projectFrom.OrderNumber,
                Notes = projectFrom.Notes,
                CustomerName = projectFrom.Customer.Name,
                Coupon = projectFrom.Coupon?.MapToCouponDto(),
                Customer = projectFrom.Customer.MapToCustomerDto(),
                Lines = projectFrom.OrderLines?.Select(x => x.MapToOrderLineDto()).ToList() ?? [],
                CustomerId = projectFrom.CustomerId,
                CouponId = projectFrom.Coupon!.Id,
                LineIds = projectFrom.OrderLines?.Select(x => x.Id).ToList() ?? [],
                ProductNames = projectFrom.OrderLines?.Select(x => x.ProductName).ToList() ?? [],
                Status = projectFrom.Status,
                StatusView = (OrderStatusDto)projectFrom.Status,
                DisplayLabel = projectFrom.GetDisplayLabel(),
                Payments = projectFrom.PaymentMethods?.Select(x => x.MapToPaymentMethodDto()).ToList() ?? [],
                CustomerCity = projectFrom.Customer.Address!.City,
                CouponPercentOff = projectFrom.Coupon!.PercentOff,
                CouponKind = projectFrom.Coupon!.Kind,
                TagNames = projectFrom.Tags?.Select(x => x.Name).ToList(),
                SrcFormLabel = projectFrom.OrderNumber + " / " + projectFrom.Status,
                ProjectFromFormLabel = projectFrom.OrderNumber + " / " + projectFrom.Status,
            };
        }

        public static List<OrderDto> MapToOrderDtoList(this IEnumerable<Order> projectFrom) => projectFrom.Select(x => x.MapToOrderDto()).ToList();
    }
}
