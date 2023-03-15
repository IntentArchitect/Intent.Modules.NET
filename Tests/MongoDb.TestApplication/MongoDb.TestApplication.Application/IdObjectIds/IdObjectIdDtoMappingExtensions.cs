using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdObjectIds
{
    public static class IdObjectIdDtoMappingExtensions
    {
        public static IdObjectIdDto MapToIdObjectIdDto(this IdObjectId projectFrom, IMapper mapper)
        {
            return mapper.Map<IdObjectIdDto>(projectFrom);
        }

        public static List<IdObjectIdDto> MapToIdObjectIdDtoList(this IEnumerable<IdObjectId> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToIdObjectIdDto(mapper)).ToList();
        }
    }
}