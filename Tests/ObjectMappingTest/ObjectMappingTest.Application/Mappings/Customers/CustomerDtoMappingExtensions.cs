using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Customers
{
    public static class CustomerDtoMappingExtensions
    {
        public static CustomerDto MapToCustomerDto(this Customer projectFrom)
        {
            return new CustomerDto
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name,
                Email = projectFrom.Email,
                Address = projectFrom.Address?.MapToAddressDto()
            };
        }

        public static List<CustomerDto> MapToCustomerDtoList(this IEnumerable<Customer> projectFrom) => projectFrom.Select(x => x.MapToCustomerDto()).ToList();
    }
}