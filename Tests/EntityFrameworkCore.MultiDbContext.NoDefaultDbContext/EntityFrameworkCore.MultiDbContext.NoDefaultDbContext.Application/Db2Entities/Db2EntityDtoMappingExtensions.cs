using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities
{
    public static class Db2EntityDtoMappingExtensions
    {
        public static Db2EntityDto MapToDb2EntityDto(this Db2Entity projectFrom, IMapper mapper)
            => mapper.Map<Db2EntityDto>(projectFrom);

        public static List<Db2EntityDto> MapToDb2EntityDtoList(this IEnumerable<Db2Entity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDb2EntityDto(mapper)).ToList();
    }
}