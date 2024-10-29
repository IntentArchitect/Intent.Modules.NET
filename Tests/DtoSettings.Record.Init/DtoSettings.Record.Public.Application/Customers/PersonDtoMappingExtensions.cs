using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DtoSettings.Record.Public.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Customers
{
    public static class PersonDtoMappingExtensions
    {
        public static PersonDto MapToPersonDto(this Person projectFrom, IMapper mapper)
            => mapper.Map<PersonDto>(projectFrom);

        public static List<PersonDto> MapToPersonDtoList(this IEnumerable<Person> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPersonDto(mapper)).ToList();
    }
}