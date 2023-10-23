using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders
{
    public static class OrderCustomerDtoMappingExtensions
    {
        public static OrderCustomerDto MapToOrderCustomerDto(this Customer projectFrom, IMapper mapper)
            => mapper.Map<OrderCustomerDto>(projectFrom);

        public static List<OrderCustomerDto> MapToOrderCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderCustomerDto(mapper)).ToList();
    }
}