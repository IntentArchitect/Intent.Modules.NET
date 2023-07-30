using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources
{
    public static class OneToManySourceDtoMappingExtensions
    {
        public static OneToManySourceDto MapToOneToManySourceDto(this OneToManySource projectFrom, IMapper mapper)
            => mapper.Map<OneToManySourceDto>(projectFrom);

        public static List<OneToManySourceDto> MapToOneToManySourceDtoList(this IEnumerable<OneToManySource> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOneToManySourceDto(mapper)).ToList();
    }
}