using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.People
{
    public static class PersonDtoMappingExtensions
    {
        public static PersonDto MapToPersonDto(this Person projectFrom, IMapper mapper)
            => mapper.Map<PersonDto>(projectFrom);

        public static List<PersonDto> MapToPersonDtoList(this IEnumerable<Person> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPersonDto(mapper)).ToList();
    }
}