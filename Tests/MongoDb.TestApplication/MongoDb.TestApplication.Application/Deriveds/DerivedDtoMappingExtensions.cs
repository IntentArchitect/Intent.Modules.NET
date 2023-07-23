using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Deriveds
{
    public static class DerivedDtoMappingExtensions
    {
        public static DerivedDto MapToDerivedDto(this Derived projectFrom, IMapper mapper)
            => mapper.Map<DerivedDto>(projectFrom);

        public static List<DerivedDto> MapToDerivedDtoList(this IEnumerable<Derived> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDerivedDto(mapper)).ToList();
    }
}