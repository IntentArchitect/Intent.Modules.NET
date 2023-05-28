using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users
{
    public static class UserAssignedPrivilegeDtoMappingExtensions
    {
        public static UserAssignedPrivilegeDto MapToUserAssignedPrivilegeDto(this AssignedPrivilege projectFrom, IMapper mapper)
            => mapper.Map<UserAssignedPrivilegeDto>(projectFrom);

        public static List<UserAssignedPrivilegeDto> MapToUserAssignedPrivilegeDtoList(this IEnumerable<AssignedPrivilege> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToUserAssignedPrivilegeDto(mapper)).ToList();
    }
}