using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities
{
    public static class TestEntityMoneyDtoMappingExtensions
    {
        public static TestEntityMoneyDto MapToTestEntityMoneyDto(this Money projectFrom, IMapper mapper)
            => mapper.Map<TestEntityMoneyDto>(projectFrom);

        public static List<TestEntityMoneyDto> MapToTestEntityMoneyDtoList(this IEnumerable<Money> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTestEntityMoneyDto(mapper)).ToList();
    }
}