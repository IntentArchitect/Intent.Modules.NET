using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers
{
    public static class CustomerAddressDtoMappingExtensions
    {
        public static CustomerAddressDto MapToCustomerAddressDto(this Address projectFrom, IMapper mapper)
            => mapper.Map<CustomerAddressDto>(projectFrom);

        public static List<CustomerAddressDto> MapToCustomerAddressDtoList(this IEnumerable<Address> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerAddressDto(mapper)).ToList();
    }
}