using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities
{
    public static class TestEntityDtoMappingExtensions
    {
        public static TestEntityDto MapToTestEntityDto(this TestEntity projectFrom, IMapper mapper)
            => mapper.Map<TestEntityDto>(projectFrom);

        public static List<TestEntityDto> MapToTestEntityDtoList(this IEnumerable<TestEntity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTestEntityDto(mapper)).ToList();
    }
}