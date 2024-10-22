using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Orders
{
    public static class OrderCountryDtoMappingExtensions
    {
        public static OrderCountryDto MapToOrderCountryDto(this Country projectFrom, IMapper mapper)
            => mapper.Map<OrderCountryDto>(projectFrom);

        public static List<OrderCountryDto> MapToOrderCountryDtoList(this IEnumerable<Country> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderCountryDto(mapper)).ToList();
    }
}