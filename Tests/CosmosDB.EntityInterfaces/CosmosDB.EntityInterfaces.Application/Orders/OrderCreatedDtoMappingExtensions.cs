using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Orders
{
    public static class OrderCreatedDtoMappingExtensions
    {
        public static OrderCreatedDto MapToOrderCreatedDto(this IOrder projectFrom, IMapper mapper)
            => mapper.Map<OrderCreatedDto>(projectFrom);

        public static List<OrderCreatedDto> MapToOrderCreatedDtoList(this IEnumerable<IOrder> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderCreatedDto(mapper)).ToList();
    }
}