using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones
{
    public static class OneFiveDtoMappingExtensions
    {
        public static OneFiveDto MapToOneFiveDto(this Five projectFrom, IMapper mapper)
            => mapper.Map<OneFiveDto>(projectFrom);

        public static List<OneFiveDto> MapToOneFiveDtoList(this IEnumerable<Five> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOneFiveDto(mapper)).ToList();
    }
}