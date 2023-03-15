using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdLongs
{
    public static class IdLongDtoMappingExtensions
    {
        public static IdLongDto MapToIdLongDto(this IdLong projectFrom, IMapper mapper)
        {
            return mapper.Map<IdLongDto>(projectFrom);
        }

        public static List<IdLongDto> MapToIdLongDtoList(this IEnumerable<IdLong> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToIdLongDto(mapper)).ToList();
        }
    }
}