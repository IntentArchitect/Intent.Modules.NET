using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests
{
    public static class OptionalToOneDestDtoMappingExtensions
    {
        public static OptionalToOneDestDto MapToOptionalToOneDestDto(this OptionalToOneDest projectFrom, IMapper mapper)
            => mapper.Map<OptionalToOneDestDto>(projectFrom);

        public static List<OptionalToOneDestDto> MapToOptionalToOneDestDtoList(this IEnumerable<OptionalToOneDest> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOptionalToOneDestDto(mapper)).ToList();
    }
}