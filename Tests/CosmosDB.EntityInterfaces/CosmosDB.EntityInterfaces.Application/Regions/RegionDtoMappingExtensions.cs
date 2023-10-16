using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Regions
{
    public static class RegionDtoMappingExtensions
    {
        public static RegionDto MapToRegionDto(this IRegion projectFrom, IMapper mapper)
            => mapper.Map<RegionDto>(projectFrom);

        public static List<RegionDto> MapToRegionDtoList(this IEnumerable<IRegion> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToRegionDto(mapper)).ToList();
    }
}