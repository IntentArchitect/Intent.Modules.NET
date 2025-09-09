using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    public static class FourDtoMappingExtensions
    {
        public static FourDto MapToFourDto(this Four projectFrom, IMapper mapper)
            => mapper.Map<FourDto>(projectFrom);

        public static List<FourDto> MapToFourDtoList(this IEnumerable<Four> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToFourDto(mapper)).ToList();
    }
}