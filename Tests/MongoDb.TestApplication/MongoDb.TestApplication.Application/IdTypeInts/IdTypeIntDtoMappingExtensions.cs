using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeInts
{
    public static class IdTypeIntDtoMappingExtensions
    {
        public static IdTypeIntDto MapToIdTypeIntDto(this IdTypeInt projectFrom, IMapper mapper)
        {
            return mapper.Map<IdTypeIntDto>(projectFrom);
        }

        public static List<IdTypeIntDto> MapToIdTypeIntDtoList(this IEnumerable<IdTypeInt> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToIdTypeIntDto(mapper)).ToList();
        }
    }
}