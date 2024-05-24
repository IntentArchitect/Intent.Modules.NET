using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public static class PersonDetailsDtoMappingExtensions
    {
        public static PersonDetailsDto MapToPersonDetailsDto(this PersonDetails projectFrom, IMapper mapper)
            => mapper.Map<PersonDetailsDto>(projectFrom);

        public static List<PersonDetailsDto> MapToPersonDetailsDtoList(this IEnumerable<PersonDetails> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPersonDetailsDto(mapper)).ToList();
    }
}