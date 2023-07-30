using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManyDests
{
    public static class ManyToManyDestDtoMappingExtensions
    {
        public static ManyToManyDestDto MapToManyToManyDestDto(this ManyToManyDest projectFrom, IMapper mapper)
            => mapper.Map<ManyToManyDestDto>(projectFrom);

        public static List<ManyToManyDestDto> MapToManyToManyDestDtoList(this IEnumerable<ManyToManyDest> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToManyToManyDestDto(mapper)).ToList();
    }
}