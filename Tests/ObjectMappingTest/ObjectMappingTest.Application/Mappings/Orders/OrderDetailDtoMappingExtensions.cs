using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Application.Customers;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public static class OrderDetailDtoMappingExtensions
    {
        public static OrderDetailDto MapToOrderDetailDto(this Order projectFrom)
        {
            return new OrderDetailDto
            {
                Id = projectFrom.Id,
                RefNo = projectFrom.RefNo,
                Status = projectFrom.Status,
                CustomerId = projectFrom.CustomerId,
                CustomerName = projectFrom.Customer.Name,
                CustomerEmail = projectFrom.Customer.Email,
                CustomerAddress = projectFrom.Customer.Address?.MapToAddressDto(),
                Lines = projectFrom.Lines?.Select(x => x.MapToOrderLineDto()).ToList() ?? [],
                TagIds = projectFrom.Tags?.Select(x => x.Id).ToList() ?? []
            };
        }

        public static List<OrderDetailDto> MapToOrderDetailDtoList(this IEnumerable<Order> projectFrom) => projectFrom.Select(x => x.MapToOrderDetailDto()).ToList();
    }
}