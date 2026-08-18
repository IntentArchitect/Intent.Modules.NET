using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Lenient.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders
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
                CouponId = projectFrom.Coupon?.Id ?? default!,
                LineIds = projectFrom.OrderLines?.Select(x => x.Id).ToList() ?? [],
                ProductNames = projectFrom.OrderLines?.Select(x => x.ProductName).ToList() ?? [],
                Status = projectFrom.Status,
                StatusView = (OrderStatusDto)projectFrom.Status,
                DisplayLabel = projectFrom.GetDisplayLabel(),
                Payments = projectFrom.PaymentMethods?.Select(x => x.MapToPaymentMethodDto()).ToList() ?? [],
                CustomerCity = projectFrom.Customer.Address?.City ?? default!,
                CouponPercentOff = projectFrom.Coupon?.PercentOff ?? default!,
                CouponKind = projectFrom.Coupon?.Kind ?? default!,
                TagNames = projectFrom.Tags?.Select(x => x.Name).ToList(),
                SrcFormLabel = projectFrom.OrderNumber + " / " + projectFrom.Status,
                ProjectFromFormLabel = projectFrom.OrderNumber + " / " + projectFrom.Status,
            };
        }

        public static List<OrderDto> MapToOrderDtoList(this IEnumerable<Order> projectFrom) => projectFrom.Select(x => x.MapToOrderDto()).ToList();
    }
}
