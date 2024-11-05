using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Departments
{
    public static class DepartmentUniversityDtoMappingExtensions
    {
        public static DepartmentUniversityDto MapToDepartmentUniversityDto(this IUniversity projectFrom, IMapper mapper)
            => mapper.Map<DepartmentUniversityDto>(projectFrom);

        public static List<DepartmentUniversityDto> MapToDepartmentUniversityDtoList(this IEnumerable<IUniversity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDepartmentUniversityDto(mapper)).ToList();
    }
}