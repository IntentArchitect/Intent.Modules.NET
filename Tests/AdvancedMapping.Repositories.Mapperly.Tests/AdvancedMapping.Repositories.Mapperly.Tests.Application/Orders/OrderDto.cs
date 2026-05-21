using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderDto
    {
        public OrderDto()
        {
            Status = null!;
            Discounts = null!;
            OrderLines = null!;
            Payments = null!;
            Shipments = null!;
            CustomerSegments = null!;
            CustomerName = null!;
            CustomerEmail = null!;
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredBy { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDiscountDto> Discounts { get; set; }
        public List<OrderOrderLineDto> OrderLines { get; set; }
        public List<OrderPaymentDto> Payments { get; set; }
        public List<OrderShipmentDto> Shipments { get; set; }
        public bool IsActive { get; set; }
        public List<OrderCustomerSegmentsDto> CustomerSegments { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public static OrderDto Create(
            Guid id,
            Guid customerId,
            DateTime orderDate,
            DateTime requiredBy,
            string status,
            decimal totalAmount,
            List<OrderDiscountDto> discounts,
            List<OrderOrderLineDto> orderLines,
            List<OrderPaymentDto> payments,
            List<OrderShipmentDto> shipments,
            bool isActive,
            List<OrderCustomerSegmentsDto> customerSegments, string customerName, string customerEmail)
        {
            return new OrderDto
            {
                Id = id,
                CustomerId = customerId,
                OrderDate = orderDate,
                RequiredBy = requiredBy,
                Status = status,
                TotalAmount = totalAmount,
                Discounts = discounts,
                OrderLines = orderLines,
                Payments = payments,
                Shipments = shipments,
                IsActive = isActive
,
                CustomerSegments = customerSegments,
                CustomerName = customerName,
                CustomerEmail = customerEmail
            };
        }
    }
}