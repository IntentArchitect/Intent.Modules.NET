using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Orders
{
    public static class OrderCurrencyDtoMappingExtensions
    {
        public static OrderCurrencyDto MapToOrderCurrencyDto(this Currency projectFrom, IMapper mapper)
            => mapper.Map<OrderCurrencyDto>(projectFrom);

        public static List<OrderCurrencyDto> MapToOrderCurrencyDtoList(this IEnumerable<Currency> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderCurrencyDto(mapper)).ToList();
    }
}