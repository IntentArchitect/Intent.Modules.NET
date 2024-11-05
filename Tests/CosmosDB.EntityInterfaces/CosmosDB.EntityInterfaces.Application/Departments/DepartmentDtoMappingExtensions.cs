using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Departments
{
    public static class DepartmentDtoMappingExtensions
    {
        public static DepartmentDto MapToDepartmentDto(this IDepartment projectFrom, IMapper mapper)
            => mapper.Map<DepartmentDto>(projectFrom);

        public static List<DepartmentDto> MapToDepartmentDtoList(this IEnumerable<IDepartment> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDepartmentDto(mapper)).ToList();
    }
}