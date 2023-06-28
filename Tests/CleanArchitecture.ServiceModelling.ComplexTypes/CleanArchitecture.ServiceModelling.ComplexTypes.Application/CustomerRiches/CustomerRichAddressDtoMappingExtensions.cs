using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches
{
    public static class CustomerRichAddressDtoMappingExtensions
    {
        public static CustomerRichAddressDto MapToCustomerRichAddressDto(this Address projectFrom, IMapper mapper)
            => mapper.Map<CustomerRichAddressDto>(projectFrom);

        public static List<CustomerRichAddressDto> MapToCustomerRichAddressDtoList(this IEnumerable<Address> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerRichAddressDto(mapper)).ToList();
    }
}