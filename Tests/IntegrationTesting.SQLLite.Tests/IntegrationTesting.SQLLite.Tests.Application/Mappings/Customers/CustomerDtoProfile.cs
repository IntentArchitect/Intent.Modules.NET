using AutoMapper;
using IntegrationTesting.SQLLite.Tests.Application.Customers;
using IntegrationTesting.SQLLite.Tests.Domain.Entities;

namespace IntegrationTesting.SQLLite.Tests.Application.Mappings.Customers
{
    public class CustomerDtoProfile : Profile
    {
        public CustomerDtoProfile()
        {
            CreateMap<Customer, CustomerDto>();
        }
    }

    public static class CustomerDtoMappingExtensions
    {
        public static CustomerDto MapToCustomerDto(this Customer projectFrom, IMapper mapper) =>
            mapper.Map<CustomerDto>(projectFrom);

        public static List<CustomerDto> MapToCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper) =>
            projectFrom.Select(x => x.MapToCustomerDto(mapper)).ToList();
    }
}
