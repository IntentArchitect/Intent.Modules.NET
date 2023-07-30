using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneSources
{
    public static class ManyToOneSourceDtoMappingExtensions
    {
        public static ManyToOneSourceDto MapToManyToOneSourceDto(this ManyToOneSource projectFrom, IMapper mapper)
            => mapper.Map<ManyToOneSourceDto>(projectFrom);

        public static List<ManyToOneSourceDto> MapToManyToOneSourceDtoList(this IEnumerable<ManyToOneSource> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToManyToOneSourceDto(mapper)).ToList();
    }
}