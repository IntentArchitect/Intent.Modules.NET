using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents
{
    public static class CompoundIndexEntityMultiParentDtoMappingExtensions
    {
        public static CompoundIndexEntityMultiParentDto MapToCompoundIndexEntityMultiParentDto(this CompoundIndexEntityMultiParent projectFrom, IMapper mapper)
            => mapper.Map<CompoundIndexEntityMultiParentDto>(projectFrom);

        public static List<CompoundIndexEntityMultiParentDto> MapToCompoundIndexEntityMultiParentDtoList(this IEnumerable<CompoundIndexEntityMultiParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCompoundIndexEntityMultiParentDto(mapper)).ToList();
    }
}