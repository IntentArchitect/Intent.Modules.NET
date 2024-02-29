using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders
{
    public static class OrderOrderLineDtoMappingExtensions
    {
        public static OrderOrderLineDto MapToOrderOrderLineDto(this OrderLine projectFrom, IMapper mapper)
            => mapper.Map<OrderOrderLineDto>(projectFrom);

        public static List<OrderOrderLineDto> MapToOrderOrderLineDtoList(this IEnumerable<OrderLine> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderOrderLineDto(mapper)).ToList();
    }
}