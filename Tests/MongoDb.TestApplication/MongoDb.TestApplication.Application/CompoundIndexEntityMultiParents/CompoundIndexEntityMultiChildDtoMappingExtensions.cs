using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents
{
    public static class CompoundIndexEntityMultiChildDtoMappingExtensions
    {
        public static CompoundIndexEntityMultiChildDto MapToCompoundIndexEntityMultiChildDto(this CompoundIndexEntityMultiChild projectFrom, IMapper mapper)
            => mapper.Map<CompoundIndexEntityMultiChildDto>(projectFrom);

        public static List<CompoundIndexEntityMultiChildDto> MapToCompoundIndexEntityMultiChildDtoList(this IEnumerable<CompoundIndexEntityMultiChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCompoundIndexEntityMultiChildDto(mapper)).ToList();
    }
}