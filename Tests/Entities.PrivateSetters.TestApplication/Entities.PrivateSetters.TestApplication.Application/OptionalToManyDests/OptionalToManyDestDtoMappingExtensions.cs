using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManyDests
{
    public static class OptionalToManyDestDtoMappingExtensions
    {
        public static OptionalToManyDestDto MapToOptionalToManyDestDto(this OptionalToManyDest projectFrom, IMapper mapper)
            => mapper.Map<OptionalToManyDestDto>(projectFrom);

        public static List<OptionalToManyDestDto> MapToOptionalToManyDestDtoList(this IEnumerable<OptionalToManyDest> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOptionalToManyDestDto(mapper)).ToList();
    }
}