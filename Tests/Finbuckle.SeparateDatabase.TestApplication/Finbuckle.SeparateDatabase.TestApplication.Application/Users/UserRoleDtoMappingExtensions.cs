using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Users
{
    public static class UserRoleDtoMappingExtensions
    {
        public static UserRoleDto MapToUserRoleDto(this Role projectFrom, IMapper mapper)
        {
            return mapper.Map<UserRoleDto>(projectFrom);
        }

        public static List<UserRoleDto> MapToUserRoleDtoList(this IEnumerable<Role> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToUserRoleDto(mapper)).ToList();
        }
    }
}