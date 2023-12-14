using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Orders
{
    public static class OrderDtoMappingExtensions
    {
        public static OrderDto MapToOrderDto(this IOrder projectFrom, IMapper mapper)
            => mapper.Map<OrderDto>(projectFrom);

        public static List<OrderDto> MapToOrderDtoList(this IEnumerable<IOrder> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderDto(mapper)).ToList();
    }
}