using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People
{
    public static class PersonPersonPersonDetailsNameDtoMappingExtensions
    {
        public static PersonPersonPersonDetailsNameDto MapToPersonPersonPersonDetailsNameDto(this Names projectFrom, IMapper mapper)
            => mapper.Map<PersonPersonPersonDetailsNameDto>(projectFrom);

        public static List<PersonPersonPersonDetailsNameDto> MapToPersonPersonPersonDetailsNameDtoList(this IEnumerable<Names> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPersonPersonPersonDetailsNameDto(mapper)).ToList();
    }
}