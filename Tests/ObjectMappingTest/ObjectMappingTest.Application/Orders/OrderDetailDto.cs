using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Application.Customers;
using ObjectMappingTest.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public record OrderDetailDto
    {
        public OrderDetailDto()
        {
            RefNo = null!;
            CustomerName = null!;
            Lines = null!;
            TagIds = null!;
        }

        public Guid Id { get; init; }
        public string RefNo { get; init; }
        public OrderStatus Status { get; init; }
        public Guid CustomerId { get; init; }
        public string CustomerName { get; init; }
        public string? CustomerEmail { get; init; }
        public AddressDto? CustomerAddress { get; init; }
        public List<OrderLineDto> Lines { get; init; }
        public List<Guid> TagIds { get; init; }
    }
}