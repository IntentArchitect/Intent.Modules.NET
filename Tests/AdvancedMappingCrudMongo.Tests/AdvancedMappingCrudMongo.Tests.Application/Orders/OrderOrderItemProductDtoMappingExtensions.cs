using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders
{
    public static class OrderOrderItemProductDtoMappingExtensions
    {
        public static OrderOrderItemProductDto MapToOrderOrderItemProductDto(this Product projectFrom, IMapper mapper)
            => mapper.Map<OrderOrderItemProductDto>(projectFrom);

        public static List<OrderOrderItemProductDto> MapToOrderOrderItemProductDtoList(this IEnumerable<Product> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderOrderItemProductDto(mapper)).ToList();
    }
}