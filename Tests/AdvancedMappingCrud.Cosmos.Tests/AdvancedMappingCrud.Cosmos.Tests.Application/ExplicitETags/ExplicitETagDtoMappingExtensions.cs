using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags
{
    public static class ExplicitETagDtoMappingExtensions
    {
        public static ExplicitETagDto MapToExplicitETagDto(this ExplicitETag projectFrom, IMapper mapper)
            => mapper.Map<ExplicitETagDto>(projectFrom);

        public static List<ExplicitETagDto> MapToExplicitETagDtoList(this IEnumerable<ExplicitETag> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToExplicitETagDto(mapper)).ToList();
    }
}