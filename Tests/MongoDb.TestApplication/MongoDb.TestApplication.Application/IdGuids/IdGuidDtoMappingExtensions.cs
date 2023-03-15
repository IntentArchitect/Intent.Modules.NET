using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdGuids
{
    public static class IdGuidDtoMappingExtensions
    {
        public static IdGuidDto MapToIdGuidDto(this IdGuid projectFrom, IMapper mapper)
        {
            return mapper.Map<IdGuidDto>(projectFrom);
        }

        public static List<IdGuidDto> MapToIdGuidDtoList(this IEnumerable<IdGuid> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToIdGuidDto(mapper)).ToList();
        }
    }
}