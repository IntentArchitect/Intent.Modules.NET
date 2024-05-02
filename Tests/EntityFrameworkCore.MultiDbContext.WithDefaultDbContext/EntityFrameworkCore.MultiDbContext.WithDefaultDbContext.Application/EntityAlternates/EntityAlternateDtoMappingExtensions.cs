using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates
{
    public static class EntityAlternateDtoMappingExtensions
    {
        public static EntityAlternateDto MapToEntityAlternateDto(this EntityAlternate projectFrom, IMapper mapper)
            => mapper.Map<EntityAlternateDto>(projectFrom);

        public static List<EntityAlternateDto> MapToEntityAlternateDtoList(this IEnumerable<EntityAlternate> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToEntityAlternateDto(mapper)).ToList();
    }
}