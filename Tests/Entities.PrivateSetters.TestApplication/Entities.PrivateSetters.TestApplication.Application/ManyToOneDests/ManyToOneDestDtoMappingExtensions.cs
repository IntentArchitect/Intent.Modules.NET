using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests
{
    public static class ManyToOneDestDtoMappingExtensions
    {
        public static ManyToOneDestDto MapToManyToOneDestDto(this ManyToOneDest projectFrom, IMapper mapper)
            => mapper.Map<ManyToOneDestDto>(projectFrom);

        public static List<ManyToOneDestDto> MapToManyToOneDestDtoList(this IEnumerable<ManyToOneDest> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToManyToOneDestDto(mapper)).ToList();
    }
}