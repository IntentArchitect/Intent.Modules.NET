using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies
{
    public static class BasicOrderByDtoMappingExtensions
    {
        public static BasicOrderByDto MapToBasicOrderByDto(this BasicOrderBy projectFrom, IMapper mapper)
            => mapper.Map<BasicOrderByDto>(projectFrom);

        public static List<BasicOrderByDto> MapToBasicOrderByDtoList(this IEnumerable<BasicOrderBy> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToBasicOrderByDto(mapper)).ToList();
    }
}