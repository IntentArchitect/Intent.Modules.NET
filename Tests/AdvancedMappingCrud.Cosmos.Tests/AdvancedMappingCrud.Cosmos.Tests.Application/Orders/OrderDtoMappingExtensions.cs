using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders
{
    public static class OrderDtoMappingExtensions
    {
        public static OrderDto MapToOrderDto(this Order projectFrom, IMapper mapper)
            => mapper.Map<OrderDto>(projectFrom);

        public static List<OrderDto> MapToOrderDtoList(this IEnumerable<Order> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderDto(mapper)).ToList();
    }
}