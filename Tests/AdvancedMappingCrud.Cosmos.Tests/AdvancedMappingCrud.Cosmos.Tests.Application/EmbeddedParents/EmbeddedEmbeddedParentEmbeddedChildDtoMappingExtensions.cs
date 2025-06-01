using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents
{
    public static class EmbeddedEmbeddedParentEmbeddedChildDtoMappingExtensions
    {
        public static EmbeddedEmbeddedParentEmbeddedChildDto MapToEmbeddedEmbeddedParentEmbeddedChildDto(this EmbeddedChild projectFrom, IMapper mapper)
            => mapper.Map<EmbeddedEmbeddedParentEmbeddedChildDto>(projectFrom);

        public static List<EmbeddedEmbeddedParentEmbeddedChildDto> MapToEmbeddedEmbeddedParentEmbeddedChildDtoList(this IEnumerable<EmbeddedChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToEmbeddedEmbeddedParentEmbeddedChildDto(mapper)).ToList();
    }
}