using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.Nullability;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities
{
    public static class TestNullablityDtoMappingExtensions
    {
        public static TestNullablityDto MapToTestNullablityDto(this TestNullablity projectFrom, IMapper mapper)
            => mapper.Map<TestNullablityDto>(projectFrom);

        public static List<TestNullablityDto> MapToTestNullablityDtoList(this IEnumerable<TestNullablity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTestNullablityDto(mapper)).ToList();
    }
}