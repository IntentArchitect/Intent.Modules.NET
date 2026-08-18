using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Strict.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMapping.Strict.Application.Orders
{
    public record OrderDto
    {
        public OrderDto()
        {
            OrderNumber = null!;
            CustomerName = null!;
            Customer = null!;
            Lines = null!;
            LineIds = null!;
            ProductNames = null!;
            DisplayLabel = null!;
            Payments = null!;
            CustomerCity = null!;
            SrcFormLabel = null!;
            ProjectFromFormLabel = null!;
        }

        public string OrderNumber { get; init; }
        public string? Notes { get; init; }
        public string CustomerName { get; init; }
        public CouponDto? Coupon { get; init; }
        public CustomerDto Customer { get; init; }
        public List<OrderLineDto> Lines { get; init; }
        public Guid CustomerId { get; init; }
        public Guid CouponId { get; init; }
        public List<Guid> LineIds { get; init; }
        public List<string> ProductNames { get; init; }
        public OrderStatus Status { get; init; }
        public OrderStatusDto StatusView { get; init; }
        public string DisplayLabel { get; init; }
        public List<PaymentMethodDto> Payments { get; init; }
        public string CustomerCity { get; init; }
        public int CouponPercentOff { get; init; }
        public CouponKind CouponKind { get; init; }
        public List<string>? TagNames { get; init; }
        public string SrcFormLabel { get; init; }
        public string ProjectFromFormLabel { get; init; }
        public string? UnmappedNote { get; init; }
    }
}