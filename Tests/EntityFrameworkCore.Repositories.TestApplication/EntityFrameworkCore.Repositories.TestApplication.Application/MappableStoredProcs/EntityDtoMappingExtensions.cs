using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs
{
    public static class EntityDtoMappingExtensions
    {
        public static EntityDto MapToEntityDto(this EntityRecord projectFrom, IMapper mapper)
            => mapper.Map<EntityDto>(projectFrom);

        public static List<EntityDto> MapToEntityDtoList(this IEnumerable<EntityRecord> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToEntityDto(mapper)).ToList();
    }
}