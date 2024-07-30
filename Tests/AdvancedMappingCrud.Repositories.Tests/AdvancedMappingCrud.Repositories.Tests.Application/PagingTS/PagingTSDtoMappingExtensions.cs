using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.PagingTS
{
    public static class PagingTSDtoMappingExtensions
    {
        public static PagingTSDto MapToPagingTSDto(this Domain.Entities.PagingTS projectFrom, IMapper mapper)
            => mapper.Map<PagingTSDto>(projectFrom);

        public static List<PagingTSDto> MapToPagingTSDtoList(this IEnumerable<Domain.Entities.PagingTS> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPagingTSDto(mapper)).ToList();
    }
}