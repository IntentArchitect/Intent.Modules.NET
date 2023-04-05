using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeOjectIdStrs
{
    public static class IdTypeOjectIdStrDtoMappingExtensions
    {
        public static IdTypeOjectIdStrDto MapToIdTypeOjectIdStrDto(this IdTypeOjectIdStr projectFrom, IMapper mapper)
        {
            return mapper.Map<IdTypeOjectIdStrDto>(projectFrom);
        }

        public static List<IdTypeOjectIdStrDto> MapToIdTypeOjectIdStrDtoList(this IEnumerable<IdTypeOjectIdStr> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToIdTypeOjectIdStrDto(mapper)).ToList();
        }
    }
}