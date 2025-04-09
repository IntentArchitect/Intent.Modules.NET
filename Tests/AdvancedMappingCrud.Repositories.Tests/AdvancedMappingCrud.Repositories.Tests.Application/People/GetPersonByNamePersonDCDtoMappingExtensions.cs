using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People
{
    public static class GetPersonByNamePersonDCDtoMappingExtensions
    {
        public static GetPersonByNamePersonDCDto MapToGetPersonByNamePersonDCDto(this PersonDC projectFrom, IMapper mapper)
            => mapper.Map<GetPersonByNamePersonDCDto>(projectFrom);

        public static List<GetPersonByNamePersonDCDto> MapToGetPersonByNamePersonDCDtoList(this IEnumerable<PersonDC> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToGetPersonByNamePersonDCDto(mapper)).ToList();
    }
}