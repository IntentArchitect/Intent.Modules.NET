using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeGuids
{
    public static class IdTypeGuidDtoMappingExtensions
    {
        public static IdTypeGuidDto MapToIdTypeGuidDto(this IdTypeGuid projectFrom, IMapper mapper)
            => mapper.Map<IdTypeGuidDto>(projectFrom);

        public static List<IdTypeGuidDto> MapToIdTypeGuidDtoList(this IEnumerable<IdTypeGuid> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToIdTypeGuidDto(mapper)).ToList();
    }
}