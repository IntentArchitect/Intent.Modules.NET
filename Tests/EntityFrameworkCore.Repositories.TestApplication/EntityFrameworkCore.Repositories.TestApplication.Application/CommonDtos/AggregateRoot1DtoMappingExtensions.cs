using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CommonDtos
{
    public static class AggregateRoot1DtoMappingExtensions
    {
        public static AggregateRoot1Dto MapToAggregateRoot1Dto(this AggregateRoot1 projectFrom, IMapper mapper)
            => mapper.Map<AggregateRoot1Dto>(projectFrom);

        public static List<AggregateRoot1Dto> MapToAggregateRoot1DtoList(this IEnumerable<AggregateRoot1> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRoot1Dto(mapper)).ToList();
    }
}