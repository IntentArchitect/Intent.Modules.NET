using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.DerivedOfTS
{
    public static class DerivedOfTDtoMappingExtensions
    {
        public static DerivedOfTDto MapToDerivedOfTDto(this DerivedOfT projectFrom, IMapper mapper)
            => mapper.Map<DerivedOfTDto>(projectFrom);

        public static List<DerivedOfTDto> MapToDerivedOfTDtoList(this IEnumerable<DerivedOfT> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDerivedOfTDto(mapper)).ToList();
    }
}