using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs
{
    public static class ODataAggDtoMappingExtensions
    {
        public static ODataAggDto MapToODataAggDto(this ODataAgg projectFrom, IMapper mapper)
            => mapper.Map<ODataAggDto>(projectFrom);

        public static List<ODataAggDto> MapToODataAggDtoList(this IEnumerable<ODataAgg> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToODataAggDto(mapper)).ToList();
    }
}