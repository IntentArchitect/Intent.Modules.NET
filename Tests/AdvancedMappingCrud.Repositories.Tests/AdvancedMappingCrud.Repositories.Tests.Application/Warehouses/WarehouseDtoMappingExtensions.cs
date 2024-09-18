using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses
{
    public static class WarehouseDtoMappingExtensions
    {
        public static WarehouseDto MapToWarehouseDto(this Warehouse projectFrom, IMapper mapper)
            => mapper.Map<WarehouseDto>(projectFrom);

        public static List<WarehouseDto> MapToWarehouseDtoList(this IEnumerable<Warehouse> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToWarehouseDto(mapper)).ToList();
    }
}