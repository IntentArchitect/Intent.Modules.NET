using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntities
{
    public static class SingleIndexEntityDtoMappingExtensions
    {
        public static SingleIndexEntityDto MapToSingleIndexEntityDto(this SingleIndexEntity projectFrom, IMapper mapper)
            => mapper.Map<SingleIndexEntityDto>(projectFrom);

        public static List<SingleIndexEntityDto> MapToSingleIndexEntityDtoList(this IEnumerable<SingleIndexEntity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSingleIndexEntityDto(mapper)).ToList();
    }
}