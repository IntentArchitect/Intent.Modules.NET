using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.Application.Orders
{
    public static class OrderCreatedDtoMappingExtensions
    {
        public static OrderCreatedDto MapToOrderCreatedDto(this Order projectFrom, IMapper mapper)
            => mapper.Map<OrderCreatedDto>(projectFrom);

        public static List<OrderCreatedDto> MapToOrderCreatedDtoList(this IEnumerable<Order> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderCreatedDto(mapper)).ToList();
    }
}