using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManySources
{
    public static class OptionalToManySourceDtoMappingExtensions
    {
        public static OptionalToManySourceDto MapToOptionalToManySourceDto(this OptionalToManySource projectFrom, IMapper mapper)
            => mapper.Map<OptionalToManySourceDto>(projectFrom);

        public static List<OptionalToManySourceDto> MapToOptionalToManySourceDtoList(this IEnumerable<OptionalToManySource> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOptionalToManySourceDto(mapper)).ToList();
    }
}