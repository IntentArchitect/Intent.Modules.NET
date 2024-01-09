using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.Interfaces.EF.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.People
{
    public static class PersonDtoMappingExtensions
    {
        public static PersonDto MapToPersonDto(this IPerson projectFrom, IMapper mapper)
            => mapper.Map<PersonDto>(projectFrom);

        public static List<PersonDto> MapToPersonDtoList(this IEnumerable<IPerson> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPersonDto(mapper)).ToList();
    }
}