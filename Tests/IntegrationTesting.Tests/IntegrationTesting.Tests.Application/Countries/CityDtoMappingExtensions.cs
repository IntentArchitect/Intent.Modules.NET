using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Countries
{
    public static class CityDtoMappingExtensions
    {
        public static CityDto MapToCityDto(this City projectFrom, IMapper mapper)
            => mapper.Map<CityDto>(projectFrom);

        public static List<CityDto> MapToCityDtoList(this IEnumerable<City> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCityDto(mapper)).ToList();
    }
}