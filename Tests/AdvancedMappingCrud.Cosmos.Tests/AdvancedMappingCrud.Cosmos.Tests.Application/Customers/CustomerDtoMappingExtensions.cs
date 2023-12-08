using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers
{
    public static class CustomerDtoMappingExtensions
    {
        public static CustomerDto MapToCustomerDto(this Customer projectFrom, IMapper mapper)
            => mapper.Map<CustomerDto>(projectFrom);

        public static List<CustomerDto> MapToCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerDto(mapper)).ToList();
    }
}