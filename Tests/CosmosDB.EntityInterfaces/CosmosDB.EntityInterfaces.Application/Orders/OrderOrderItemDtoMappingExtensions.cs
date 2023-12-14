using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Orders
{
    public static class OrderOrderItemDtoMappingExtensions
    {
        public static OrderOrderItemDto MapToOrderOrderItemDto(this IOrderItem projectFrom, IMapper mapper)
            => mapper.Map<OrderOrderItemDto>(projectFrom);

        public static List<OrderOrderItemDto> MapToOrderOrderItemDtoList(this IEnumerable<IOrderItem> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderOrderItemDto(mapper)).ToList();
    }
}