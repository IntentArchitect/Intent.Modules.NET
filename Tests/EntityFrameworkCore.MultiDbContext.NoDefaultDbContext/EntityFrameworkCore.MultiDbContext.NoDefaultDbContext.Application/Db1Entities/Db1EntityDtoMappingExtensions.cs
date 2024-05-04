using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities
{
    public static class Db1EntityDtoMappingExtensions
    {
        public static Db1EntityDto MapToDb1EntityDto(this Db1Entity projectFrom, IMapper mapper)
            => mapper.Map<Db1EntityDto>(projectFrom);

        public static List<Db1EntityDto> MapToDb1EntityDtoList(this IEnumerable<Db1Entity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDb1EntityDto(mapper)).ToList();
    }
}