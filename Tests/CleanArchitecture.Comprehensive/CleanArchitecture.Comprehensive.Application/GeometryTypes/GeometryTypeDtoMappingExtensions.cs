using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.Geometry;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes
{
    public static class GeometryTypeDtoMappingExtensions
    {
        public static GeometryTypeDto MapToGeometryTypeDto(this GeometryType projectFrom, IMapper mapper)
            => mapper.Map<GeometryTypeDto>(projectFrom);

        public static List<GeometryTypeDto> MapToGeometryTypeDtoList(this IEnumerable<GeometryType> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToGeometryTypeDto(mapper)).ToList();
    }
}