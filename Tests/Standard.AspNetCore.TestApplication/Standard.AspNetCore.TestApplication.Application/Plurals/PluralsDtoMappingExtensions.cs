using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Plurals
{
    public static class PluralsDtoMappingExtensions
    {
        public static PluralsDto MapToPluralsDto(this Domain.Entities.Plurals projectFrom, IMapper mapper)
            => mapper.Map<PluralsDto>(projectFrom);

        public static List<PluralsDto> MapToPluralsDtoList(this IEnumerable<Domain.Entities.Plurals> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPluralsDto(mapper)).ToList();
    }
}