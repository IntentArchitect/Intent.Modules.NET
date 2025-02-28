using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums
{
    public static class EntityListEnumDtoMappingExtensions
    {
        public static EntityListEnumDto MapToEntityListEnumDto(this EntityListEnum projectFrom, IMapper mapper)
            => mapper.Map<EntityListEnumDto>(projectFrom);

        public static List<EntityListEnumDto> MapToEntityListEnumDtoList(this IEnumerable<EntityListEnum> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToEntityListEnumDto(mapper)).ToList();
    }
}