using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public static class UserUserAddressDtoMappingExtensions
    {
        public static UserUserAddressDto MapToUserUserAddressDto(this Address projectFrom, IMapper mapper)
            => mapper.Map<UserUserAddressDto>(projectFrom);

        public static List<UserUserAddressDto> MapToUserUserAddressDtoList(this IEnumerable<Address> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToUserUserAddressDto(mapper)).ToList();
    }
}