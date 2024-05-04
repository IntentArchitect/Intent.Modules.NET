using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults
{
    public static class EntityAppDefaultDtoMappingExtensions
    {
        public static EntityAppDefaultDto MapToEntityAppDefaultDto(this EntityAppDefault projectFrom, IMapper mapper)
            => mapper.Map<EntityAppDefaultDto>(projectFrom);

        public static List<EntityAppDefaultDto> MapToEntityAppDefaultDtoList(this IEnumerable<EntityAppDefault> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToEntityAppDefaultDto(mapper)).ToList();
    }
}