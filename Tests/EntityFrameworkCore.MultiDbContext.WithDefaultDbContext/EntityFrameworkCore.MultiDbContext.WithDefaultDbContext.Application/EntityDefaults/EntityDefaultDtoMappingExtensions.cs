using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults
{
    public static class EntityDefaultDtoMappingExtensions
    {
        public static EntityDefaultDto MapToEntityDefaultDto(this EntityDefault projectFrom, IMapper mapper)
            => mapper.Map<EntityDefaultDto>(projectFrom);

        public static List<EntityDefaultDto> MapToEntityDefaultDtoList(this IEnumerable<EntityDefault> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToEntityDefaultDto(mapper)).ToList();
    }
}