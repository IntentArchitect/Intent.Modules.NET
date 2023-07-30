using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources
{
    public static class OneToManySourceOneToManyDestDtoMappingExtensions
    {
        public static OneToManySourceOneToManyDestDto MapToOneToManySourceOneToManyDestDto(this OneToManyDest projectFrom, IMapper mapper)
            => mapper.Map<OneToManySourceOneToManyDestDto>(projectFrom);

        public static List<OneToManySourceOneToManyDestDto> MapToOneToManySourceOneToManyDestDtoList(this IEnumerable<OneToManyDest> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOneToManySourceOneToManyDestDto(mapper)).ToList();
    }
}