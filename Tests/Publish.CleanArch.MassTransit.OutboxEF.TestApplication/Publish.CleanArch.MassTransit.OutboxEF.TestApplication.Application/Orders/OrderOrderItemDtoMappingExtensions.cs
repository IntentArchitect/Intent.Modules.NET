using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Orders
{
    public static class OrderOrderItemDtoMappingExtensions
    {
        public static OrderOrderItemDto MapToOrderOrderItemDto(this OrderItem projectFrom, IMapper mapper)
            => mapper.Map<OrderOrderItemDto>(projectFrom);

        public static List<OrderOrderItemDto> MapToOrderOrderItemDtoList(this IEnumerable<OrderItem> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderOrderItemDto(mapper)).ToList();
    }
}