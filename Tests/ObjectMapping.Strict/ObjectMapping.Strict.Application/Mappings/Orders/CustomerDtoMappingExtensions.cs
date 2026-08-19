using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Strict.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMapping.Strict.Application.Orders
{
    public static class CustomerDtoMappingExtensions
    {
        public static CustomerDto MapToCustomerDto(this Customer projectFrom)
        {
            return new CustomerDto
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name,
                Tier = projectFrom.Tier
            };
        }

        public static List<CustomerDto> MapToCustomerDtoList(this IEnumerable<Customer> projectFrom) => projectFrom.Select(x => x.MapToCustomerDto()).ToList();
    }
}