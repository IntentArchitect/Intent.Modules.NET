using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.DiffIds
{
    public static class DiffIdDtoMappingExtensions
    {
        public static DiffIdDto MapToDiffIdDto(this DiffId projectFrom, IMapper mapper)
            => mapper.Map<DiffIdDto>(projectFrom);

        public static List<DiffIdDto> MapToDiffIdDtoList(this IEnumerable<DiffId> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDiffIdDto(mapper)).ToList();
    }
}