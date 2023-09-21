using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.AggregateWithUniqueConstraintIndexElements
{
    public static class AggregateWithUniqueConstraintIndexElementDtoMappingExtensions
    {
        public static AggregateWithUniqueConstraintIndexElementDto MapToAggregateWithUniqueConstraintIndexElementDto(this AggregateWithUniqueConstraintIndexElement projectFrom, IMapper mapper)
            => mapper.Map<AggregateWithUniqueConstraintIndexElementDto>(projectFrom);

        public static List<AggregateWithUniqueConstraintIndexElementDto> MapToAggregateWithUniqueConstraintIndexElementDtoList(this IEnumerable<AggregateWithUniqueConstraintIndexElement> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateWithUniqueConstraintIndexElementDto(mapper)).ToList();
    }
}