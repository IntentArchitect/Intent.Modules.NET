using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics
{
    public static class CustomerAnemicAddressDtoMappingExtensions
    {
        public static CustomerAnemicAddressDto MapToCustomerAnemicAddressDto(this Address projectFrom, IMapper mapper)
            => mapper.Map<CustomerAnemicAddressDto>(projectFrom);

        public static List<CustomerAnemicAddressDto> MapToCustomerAnemicAddressDtoList(this IEnumerable<Address> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerAnemicAddressDto(mapper)).ToList();
    }
}