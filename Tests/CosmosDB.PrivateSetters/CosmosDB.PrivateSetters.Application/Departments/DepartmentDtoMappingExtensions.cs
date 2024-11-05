using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.Departments
{
    public static class DepartmentDtoMappingExtensions
    {
        public static DepartmentDto MapToDepartmentDto(this Department projectFrom, IMapper mapper)
            => mapper.Map<DepartmentDto>(projectFrom);

        public static List<DepartmentDto> MapToDepartmentDtoList(this IEnumerable<Department> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDepartmentDto(mapper)).ToList();
    }
}