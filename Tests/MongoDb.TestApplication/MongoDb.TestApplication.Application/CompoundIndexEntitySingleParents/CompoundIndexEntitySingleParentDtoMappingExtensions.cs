using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents
{
    public static class CompoundIndexEntitySingleParentDtoMappingExtensions
    {
        public static CompoundIndexEntitySingleParentDto MapToCompoundIndexEntitySingleParentDto(this CompoundIndexEntitySingleParent projectFrom, IMapper mapper)
            => mapper.Map<CompoundIndexEntitySingleParentDto>(projectFrom);

        public static List<CompoundIndexEntitySingleParentDto> MapToCompoundIndexEntitySingleParentDtoList(this IEnumerable<CompoundIndexEntitySingleParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCompoundIndexEntitySingleParentDto(mapper)).ToList();
    }
}