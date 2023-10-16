using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.DerivedOfTS
{
    public static class DerivedOfTDtoMappingExtensions
    {
        public static DerivedOfTDto MapToDerivedOfTDto(this IDerivedOfT projectFrom, IMapper mapper)
            => mapper.Map<DerivedOfTDto>(projectFrom);

        public static List<DerivedOfTDto> MapToDerivedOfTDtoList(this IEnumerable<IDerivedOfT> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDerivedOfTDto(mapper)).ToList();
    }
}