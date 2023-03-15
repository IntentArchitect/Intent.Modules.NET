using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdInts
{
    public static class IdIntDtoMappingExtensions
    {
        public static IdIntDto MapToIdIntDto(this IdInt projectFrom, IMapper mapper)
        {
            return mapper.Map<IdIntDto>(projectFrom);
        }

        public static List<IdIntDto> MapToIdIntDtoList(this IEnumerable<IdInt> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToIdIntDto(mapper)).ToList();
        }
    }
}