using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users
{
    public static class UserAddressDtoMappingExtensions
    {
        public static UserAddressDto MapToUserAddressDto(this UserAddress projectFrom, IMapper mapper)
            => mapper.Map<UserAddressDto>(projectFrom);

        public static List<UserAddressDto> MapToUserAddressDtoList(this IEnumerable<UserAddress> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToUserAddressDto(mapper)).ToList();
    }
}