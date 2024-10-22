using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Countries
{
    public static class CountryDtoMappingExtensions
    {
        public static CountryDto MapToCountryDto(this Country projectFrom, IMapper mapper)
            => mapper.Map<CountryDto>(projectFrom);

        public static List<CountryDto> MapToCountryDtoList(this IEnumerable<Country> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCountryDto(mapper)).ToList();
    }
}