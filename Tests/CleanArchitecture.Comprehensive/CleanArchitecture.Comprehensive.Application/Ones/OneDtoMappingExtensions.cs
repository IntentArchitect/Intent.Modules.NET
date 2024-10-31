using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    public static class OneDtoMappingExtensions
    {
        public static OneDto MapToOneDto(this One projectFrom, IMapper mapper)
            => mapper.Map<OneDto>(projectFrom);

        public static List<OneDto> MapToOneDtoList(this IEnumerable<One> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOneDto(mapper)).ToList();
    }
}