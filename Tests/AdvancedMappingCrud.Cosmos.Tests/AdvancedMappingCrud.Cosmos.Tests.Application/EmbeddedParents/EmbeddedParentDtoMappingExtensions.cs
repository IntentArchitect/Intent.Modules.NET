using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents
{
    public static class EmbeddedParentDtoMappingExtensions
    {
        public static EmbeddedParentDto MapToEmbeddedParentDto(this EmbeddedParent projectFrom, IMapper mapper)
            => mapper.Map<EmbeddedParentDto>(projectFrom);

        public static List<EmbeddedParentDto> MapToEmbeddedParentDtoList(this IEnumerable<EmbeddedParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToEmbeddedParentDto(mapper)).ToList();
    }
}