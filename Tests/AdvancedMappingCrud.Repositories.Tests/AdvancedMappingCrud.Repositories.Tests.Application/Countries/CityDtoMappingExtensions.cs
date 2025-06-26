using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
{
    public static class CityDtoMappingExtensions
    {
        public static CityDto MapToCityDto(this City projectFrom, IMapper mapper)
            => mapper.Map<CityDto>(projectFrom);

        public static List<CityDto> MapToCityDtoList(this IEnumerable<City> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCityDto(mapper)).ToList();
    }
}