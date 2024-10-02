using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace SqlServerImporterTests.Application.Orders
{
    public static class CustomerOrderDtoMappingExtensions
    {
        public static CustomerOrderDto MapToCustomerOrderDto(this GetCustomerOrdersResponse projectFrom, IMapper mapper)
            => mapper.Map<CustomerOrderDto>(projectFrom);

        public static List<CustomerOrderDto> MapToCustomerOrderDtoList(this IEnumerable<GetCustomerOrdersResponse> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCustomerOrderDto(mapper)).ToList();
    }
}