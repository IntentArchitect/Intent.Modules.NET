using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Supers
{
    public static class SuperDtoMappingExtensions
    {
        public static SuperDto MapToSuperDto(this Super projectFrom, IMapper mapper)
            => mapper.Map<SuperDto>(projectFrom);

        public static List<SuperDto> MapToSuperDtoList(this IEnumerable<Super> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSuperDto(mapper)).ToList();
    }
}