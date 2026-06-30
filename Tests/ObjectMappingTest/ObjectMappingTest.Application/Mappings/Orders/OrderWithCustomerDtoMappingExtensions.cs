using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Application.Customers;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public static class OrderWithCustomerDtoMappingExtensions
    {
        public static OrderWithCustomerDto MapToOrderWithCustomerDto(this Order projectFrom)
        {
            return new OrderWithCustomerDto
            {
                Id = projectFrom.Id,
                RefNo = projectFrom.RefNo,
                Status = projectFrom.Status,
                Customer = projectFrom.Customer.MapToCustomerDto(),
                Lines = projectFrom.Lines?.Select(x => x.MapToOrderLineDto()).ToList() ?? [],
                Tags = projectFrom.Tags?.Select(x => x.MapToTagDto()).ToList() ?? []
            };
        }

        public static List<OrderWithCustomerDto> MapToOrderWithCustomerDtoList(this IEnumerable<Order> projectFrom) => projectFrom.Select(x => x.MapToOrderWithCustomerDto()).ToList();
    }
}