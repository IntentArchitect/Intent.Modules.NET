using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds
{
    public static class CNCCChildDtoMappingExtensions
    {
        public static CNCCChildDto MapToCNCCChildDto(this CNCCChild projectFrom, IMapper mapper)
            => mapper.Map<CNCCChildDto>(projectFrom);

        public static List<CNCCChildDto> MapToCNCCChildDtoList(this IEnumerable<CNCCChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCNCCChildDto(mapper)).ToList();
    }
}