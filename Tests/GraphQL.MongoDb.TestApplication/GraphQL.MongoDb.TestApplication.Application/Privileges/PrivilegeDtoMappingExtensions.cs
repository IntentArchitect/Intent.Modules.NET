using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges
{
    public static class PrivilegeDtoMappingExtensions
    {
        public static PrivilegeDto MapToPrivilegeDto(this Privilege projectFrom, IMapper mapper)
        {
            return mapper.Map<PrivilegeDto>(projectFrom);
        }

        public static List<PrivilegeDto> MapToPrivilegeDtoList(this IEnumerable<Privilege> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToPrivilegeDto(mapper)).ToList();
        }
    }
}