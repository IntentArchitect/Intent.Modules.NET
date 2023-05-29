using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents
{
    public static class CompoundIndexEntitySingleChildDtoMappingExtensions
    {
        public static CompoundIndexEntitySingleChildDto MapToCompoundIndexEntitySingleChildDto(this CompoundIndexEntitySingleChild projectFrom, IMapper mapper)
            => mapper.Map<CompoundIndexEntitySingleChildDto>(projectFrom);

        public static List<CompoundIndexEntitySingleChildDto> MapToCompoundIndexEntitySingleChildDtoList(this IEnumerable<CompoundIndexEntitySingleChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCompoundIndexEntitySingleChildDto(mapper)).ToList();
    }
}