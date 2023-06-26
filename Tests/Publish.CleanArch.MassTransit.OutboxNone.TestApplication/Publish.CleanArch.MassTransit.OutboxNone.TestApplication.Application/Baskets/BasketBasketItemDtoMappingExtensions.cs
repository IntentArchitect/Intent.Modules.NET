using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets
{
    public static class BasketBasketItemDtoMappingExtensions
    {
        public static BasketBasketItemDto MapToBasketBasketItemDto(this BasketItem projectFrom, IMapper mapper)
            => mapper.Map<BasketBasketItemDto>(projectFrom);

        public static List<BasketBasketItemDto> MapToBasketBasketItemDtoList(this IEnumerable<BasketItem> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToBasketBasketItemDto(mapper)).ToList();
    }
}