using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Pagination
{
    public static class PersonEntryDtoMappingExtensions
    {
        public static PersonEntryDto MapToPersonEntryDto(this PersonEntry projectFrom, IMapper mapper)
            => mapper.Map<PersonEntryDto>(projectFrom);

        public static List<PersonEntryDto> MapToPersonEntryDtoList(this IEnumerable<PersonEntry> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPersonEntryDto(mapper)).ToList();
    }
}