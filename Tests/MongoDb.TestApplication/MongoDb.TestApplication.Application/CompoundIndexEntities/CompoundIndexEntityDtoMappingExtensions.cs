using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntities
{
    public static class CompoundIndexEntityDtoMappingExtensions
    {
        public static CompoundIndexEntityDto MapToCompoundIndexEntityDto(this CompoundIndexEntity projectFrom, IMapper mapper)
            => mapper.Map<CompoundIndexEntityDto>(projectFrom);

        public static List<CompoundIndexEntityDto> MapToCompoundIndexEntityDtoList(this IEnumerable<CompoundIndexEntity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCompoundIndexEntityDto(mapper)).ToList();
    }
}