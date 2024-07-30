using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics
{
    public static class BasicDtoMappingExtensions
    {
        public static BasicDto MapToBasicDto(this Basic projectFrom, IMapper mapper)
            => mapper.Map<BasicDto>(projectFrom);

        public static List<BasicDto> MapToBasicDtoList(this IEnumerable<Basic> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToBasicDto(mapper)).ToList();
    }
}