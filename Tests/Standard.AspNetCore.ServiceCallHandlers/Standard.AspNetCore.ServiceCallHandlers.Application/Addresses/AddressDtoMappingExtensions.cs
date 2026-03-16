using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Addresses
{
    public static class AddressDtoMappingExtensions
    {
        public static AddressDto MapToAddressDto(this Address projectFrom, IMapper mapper)
            => mapper.Map<AddressDto>(projectFrom);

        public static List<AddressDto> MapToAddressDtoList(this IEnumerable<Address> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAddressDto(mapper)).ToList();
    }
}