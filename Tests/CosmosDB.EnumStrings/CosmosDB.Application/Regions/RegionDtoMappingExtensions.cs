using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.Application.Regions
{
    public static class RegionDtoMappingExtensions
    {
        public static RegionDto MapToRegionDto(this Region projectFrom, IMapper mapper)
            => mapper.Map<RegionDto>(projectFrom);

        public static List<RegionDto> MapToRegionDtoList(this IEnumerable<Region> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToRegionDto(mapper)).ToList();
    }
}