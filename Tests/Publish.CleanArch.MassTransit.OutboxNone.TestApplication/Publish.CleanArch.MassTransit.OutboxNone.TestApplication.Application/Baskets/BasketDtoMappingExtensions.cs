using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets
{
    public static class BasketDtoMappingExtensions
    {
        public static BasketDto MapToBasketDto(this Basket projectFrom, IMapper mapper)
            => mapper.Map<BasketDto>(projectFrom);

        public static List<BasketDto> MapToBasketDtoList(this IEnumerable<Basket> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToBasketDto(mapper)).ToList();
    }
}