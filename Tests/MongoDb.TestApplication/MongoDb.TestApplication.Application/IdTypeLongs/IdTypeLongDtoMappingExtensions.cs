using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeLongs
{
    public static class IdTypeLongDtoMappingExtensions
    {
        public static IdTypeLongDto MapToIdTypeLongDto(this IdTypeLong projectFrom, IMapper mapper)
        {
            return mapper.Map<IdTypeLongDto>(projectFrom);
        }

        public static List<IdTypeLongDto> MapToIdTypeLongDtoList(this IEnumerable<IdTypeLong> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToIdTypeLongDto(mapper)).ToList();
        }
    }
}