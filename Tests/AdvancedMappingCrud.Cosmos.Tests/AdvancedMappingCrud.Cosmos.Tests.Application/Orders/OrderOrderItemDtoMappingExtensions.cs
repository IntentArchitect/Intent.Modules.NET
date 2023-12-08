using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders
{
    public static class OrderOrderItemDtoMappingExtensions
    {
        public static OrderOrderItemDto MapToOrderOrderItemDto(this OrderItem projectFrom, IMapper mapper)
            => mapper.Map<OrderOrderItemDto>(projectFrom);

        public static List<OrderOrderItemDto> MapToOrderOrderItemDtoList(this IEnumerable<OrderItem> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderOrderItemDto(mapper)).ToList();
    }
}