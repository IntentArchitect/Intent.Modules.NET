using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Application.Customers;
using ObjectMappingTest.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public record OrderWithCustomerDto
    {
        public OrderWithCustomerDto()
        {
            RefNo = null!;
            Customer = null!;
            Lines = null!;
            Tags = null!;
        }

        public Guid Id { get; init; }
        public string RefNo { get; init; }
        public OrderStatus Status { get; init; }
        public CustomerDto Customer { get; init; }
        public List<OrderLineDto> Lines { get; init; }
        public List<TagDto> Tags { get; init; }
    }
}