using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Finbuckle.SharedDatabase.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Application.Users
{
    public static class UserRoleDtoMappingExtensions
    {
        public static UserRoleDto MapToUserRoleDto(this Role projectFrom, IMapper mapper)
            => mapper.Map<UserRoleDto>(projectFrom);

        public static List<UserRoleDto> MapToUserRoleDtoList(this IEnumerable<Role> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToUserRoleDto(mapper)).ToList();
    }
}