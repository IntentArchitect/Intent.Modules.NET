using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public static class PersonDetailsNameDtoMappingExtensions
    {
        public static PersonDetailsNameDto MapToPersonDetailsNameDto(this Names projectFrom, IMapper mapper)
            => mapper.Map<PersonDetailsNameDto>(projectFrom);

        public static List<PersonDetailsNameDto> MapToPersonDetailsNameDtoList(this IEnumerable<Names> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPersonDetailsNameDto(mapper)).ToList();
    }
}