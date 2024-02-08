using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Parents
{
    public static class ParentDtoMappingExtensions
    {
        public static ParentDto MapToParentDto(this Parent projectFrom, IMapper mapper)
            => mapper.Map<ParentDto>(projectFrom);

        public static List<ParentDto> MapToParentDtoList(this IEnumerable<Parent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToParentDto(mapper)).ToList();
    }
}