using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint
{
    public static class AggregateWithUniqueConstraintIndexStereotypeDtoMappingExtensions
    {
        public static AggregateWithUniqueConstraintIndexStereotypeDto MapToAggregateWithUniqueConstraintIndexStereotypeDto(this AggregateWithUniqueConstraintIndexStereotype projectFrom, IMapper mapper)
            => mapper.Map<AggregateWithUniqueConstraintIndexStereotypeDto>(projectFrom);

        public static List<AggregateWithUniqueConstraintIndexStereotypeDto> MapToAggregateWithUniqueConstraintIndexStereotypeDtoList(this IEnumerable<AggregateWithUniqueConstraintIndexStereotype> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateWithUniqueConstraintIndexStereotypeDto(mapper)).ToList();
    }
}