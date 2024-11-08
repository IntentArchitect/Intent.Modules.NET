using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Customers
{
    public static class CustomerLookupDtoMappingExtensions
    {
        public static CustomerLookupDto MapToCustomerLookupDto(this Customer projectFrom, IMapper mapper)
            => mapper.Map<CustomerLookupDto>(projectFrom);

        public static List<CustomerLookupDto> MapToCustomerLookupDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerLookupDto(mapper)).ToList();
    }
}