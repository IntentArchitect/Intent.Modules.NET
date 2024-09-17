using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.People
{
    public static class PersonPersonPersonDetailsDtoMappingExtensions
    {
        public static PersonPersonPersonDetailsDto MapToPersonPersonPersonDetailsDto(this PersonDetails projectFrom, IMapper mapper)
            => mapper.Map<PersonPersonPersonDetailsDto>(projectFrom);

        public static List<PersonPersonPersonDetailsDto> MapToPersonPersonPersonDetailsDtoList(this IEnumerable<PersonDetails> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPersonPersonPersonDetailsDto(mapper)).ToList();
    }
}