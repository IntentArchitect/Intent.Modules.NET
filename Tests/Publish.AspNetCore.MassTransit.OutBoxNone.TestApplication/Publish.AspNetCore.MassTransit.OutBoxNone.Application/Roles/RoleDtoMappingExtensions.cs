using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Application.Roles
{
    public static class RoleDtoMappingExtensions
    {
        public static RoleDto MapToRoleDto(this Role projectFrom, IMapper mapper)
            => mapper.Map<RoleDto>(projectFrom);

        public static List<RoleDto> MapToRoleDtoList(this IEnumerable<Role> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToRoleDto(mapper)).ToList();
    }
}