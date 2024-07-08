using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders
{
    public static class OrderCustomerDtoMappingExtensions
    {
        public static OrderCustomerDto MapToOrderCustomerDto(this Customer projectFrom, IMapper mapper)
            => mapper.Map<OrderCustomerDto>(projectFrom);

        public static List<OrderCustomerDto> MapToOrderCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToOrderCustomerDto(mapper)).ToList();
    }
}