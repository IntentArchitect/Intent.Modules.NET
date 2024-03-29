using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application
{
    public static class LineDtoMappingExtensions
    {
        public static LineDto MapToLineDto(this Line projectFrom, IMapper mapper)
            => mapper.Map<LineDto>(projectFrom);

        public static List<LineDto> MapToLineDtoList(this IEnumerable<Line> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToLineDto(mapper)).ToList();
    }
}