using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Children
{
    public static class ChildDtoMappingExtensions
    {
        public static ChildDto MapToChildDto(this Child projectFrom, IMapper mapper)
            => mapper.Map<ChildDto>(projectFrom);

        public static List<ChildDto> MapToChildDtoList(this IEnumerable<Child> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToChildDto(mapper)).ToList();
    }
}