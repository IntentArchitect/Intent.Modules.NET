using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.General;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.General
{
    public static class CustomMappingDtoMappingExtensions
    {
        public static CustomMappingDto MapToCustomMappingDto(this CustomMapping projectFrom, IMapper mapper)
            => mapper.Map<CustomMappingDto>(projectFrom);

        public static List<CustomMappingDto> MapToCustomMappingDtoList(this IEnumerable<CustomMapping> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomMappingDto(mapper)).ToList();
    }
}