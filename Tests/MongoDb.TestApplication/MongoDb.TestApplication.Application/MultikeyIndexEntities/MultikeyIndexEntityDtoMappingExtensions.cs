using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntities
{
    public static class MultikeyIndexEntityDtoMappingExtensions
    {
        public static MultikeyIndexEntityDto MapToMultikeyIndexEntityDto(this MultikeyIndexEntity projectFrom, IMapper mapper)
            => mapper.Map<MultikeyIndexEntityDto>(projectFrom);

        public static List<MultikeyIndexEntityDto> MapToMultikeyIndexEntityDtoList(this IEnumerable<MultikeyIndexEntity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMultikeyIndexEntityDto(mapper)).ToList();
    }
}