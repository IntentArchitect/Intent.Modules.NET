using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManySources
{
    public static class ManyToManySourceDtoMappingExtensions
    {
        public static ManyToManySourceDto MapToManyToManySourceDto(this ManyToManySource projectFrom, IMapper mapper)
            => mapper.Map<ManyToManySourceDto>(projectFrom);

        public static List<ManyToManySourceDto> MapToManyToManySourceDtoList(this IEnumerable<ManyToManySource> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToManyToManySourceDto(mapper)).ToList();
    }
}