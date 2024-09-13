using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace SqlServerImporterTests.Application.Orders
{
    public static class CustomerOrderDtoMappingExtensions
    {
        public static CustomerOrderDto MapToCustomerOrderDto(this Order projectFrom, IMapper mapper)
            => mapper.Map<CustomerOrderDto>(projectFrom);

        public static List<CustomerOrderDto> MapToCustomerOrderDtoList(this IEnumerable<Order> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerOrderDto(mapper)).ToList();
    }
}