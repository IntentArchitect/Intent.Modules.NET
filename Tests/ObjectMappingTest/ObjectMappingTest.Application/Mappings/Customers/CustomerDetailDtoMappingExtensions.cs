using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Customers
{
    public static class CustomerDetailDtoMappingExtensions
    {
        public static CustomerDetailDto MapToCustomerDetailDto(this Customer projectFrom)
        {
            return new CustomerDetailDto
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name,
                Email = projectFrom.Email,
                Address = projectFrom.Address?.MapToAddressDto(),
                ShippingAddress = projectFrom.ShippingAddress?.MapToAddressDto()
            };
        }

        public static List<CustomerDetailDto> MapToCustomerDetailDtoList(this IEnumerable<Customer> projectFrom) => projectFrom.Select(x => x.MapToCustomerDetailDto()).ToList();
    }
}