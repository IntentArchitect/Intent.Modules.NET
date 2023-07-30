using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOptionalSources
{
    public static class OneToOptionalSourceDtoMappingExtensions
    {
        public static OneToOptionalSourceDto MapToOneToOptionalSourceDto(this OneToOptionalSource projectFrom, IMapper mapper)
            => mapper.Map<OneToOptionalSourceDto>(projectFrom);

        public static List<OneToOptionalSourceDto> MapToOneToOptionalSourceDtoList(this IEnumerable<OneToOptionalSource> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOneToOptionalSourceDto(mapper)).ToList();
    }
}