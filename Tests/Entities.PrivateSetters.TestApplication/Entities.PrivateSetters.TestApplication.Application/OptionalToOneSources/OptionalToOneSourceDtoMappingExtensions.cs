using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources
{
    public static class OptionalToOneSourceDtoMappingExtensions
    {
        public static OptionalToOneSourceDto MapToOptionalToOneSourceDto(this OptionalToOneSource projectFrom, IMapper mapper)
            => mapper.Map<OptionalToOneSourceDto>(projectFrom);

        public static List<OptionalToOneSourceDto> MapToOptionalToOneSourceDtoList(this IEnumerable<OptionalToOneSource> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOptionalToOneSourceDto(mapper)).ToList();
    }
}