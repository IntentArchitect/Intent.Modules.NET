using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOneSources
{
    public static class OneToOneSourceDtoMappingExtensions
    {
        public static OneToOneSourceDto MapToOneToOneSourceDto(this OneToOneSource projectFrom, IMapper mapper)
            => mapper.Map<OneToOneSourceDto>(projectFrom);

        public static List<OneToOneSourceDto> MapToOneToOneSourceDtoList(this IEnumerable<OneToOneSource> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOneToOneSourceDto(mapper)).ToList();
    }
}