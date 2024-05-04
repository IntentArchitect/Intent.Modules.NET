using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds
{
    public static class DerivedDtoMappingExtensions
    {
        public static DerivedDto MapToDerivedDto(this Derived projectFrom, IMapper mapper)
            => mapper.Map<DerivedDto>(projectFrom);

        public static List<DerivedDto> MapToDerivedDtoList(this IEnumerable<Derived> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDerivedDto(mapper)).ToList();
    }
}