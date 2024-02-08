using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes
{
    public static class DerivedTypeDtoMappingExtensions
    {
        public static DerivedTypeDto MapToDerivedTypeDto(this DerivedType projectFrom, IMapper mapper)
            => mapper.Map<DerivedTypeDto>(projectFrom);

        public static List<DerivedTypeDto> MapToDerivedTypeDtoList(this IEnumerable<DerivedType> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDerivedTypeDto(mapper)).ToList();
    }
}