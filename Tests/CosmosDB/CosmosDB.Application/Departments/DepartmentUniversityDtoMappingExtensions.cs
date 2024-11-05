using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.Application.Departments
{
    public static class DepartmentUniversityDtoMappingExtensions
    {
        public static DepartmentUniversityDto MapToDepartmentUniversityDto(this University projectFrom, IMapper mapper)
            => mapper.Map<DepartmentUniversityDto>(projectFrom);

        public static List<DepartmentUniversityDto> MapToDepartmentUniversityDtoList(this IEnumerable<University> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDepartmentUniversityDto(mapper)).ToList();
    }
}