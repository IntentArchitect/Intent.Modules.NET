using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Contracts.DataContracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.DataContracts
{
    public static class DerivedDtoMappingExtensions
    {
        public static DerivedDto MapToDerivedDto(this DerivedDataContract projectFrom, IMapper mapper)
            => mapper.Map<DerivedDto>(projectFrom);

        public static List<DerivedDto> MapToDerivedDtoList(this IEnumerable<DerivedDataContract> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDerivedDto(mapper)).ToList();
    }
}