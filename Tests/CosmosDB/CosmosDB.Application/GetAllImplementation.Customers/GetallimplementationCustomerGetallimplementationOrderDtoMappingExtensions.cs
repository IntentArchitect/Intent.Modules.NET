using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.Application.GetAllImplementation.Customers
{
    public static class GetallimplementationCustomerGetallimplementationOrderDtoMappingExtensions
    {
        public static GetallimplementationCustomerGetallimplementationOrderDto MapToGetallimplementationCustomerGetallimplementationOrderDto(this GetAllImplementationOrder projectFrom, IMapper mapper)
            => mapper.Map<GetallimplementationCustomerGetallimplementationOrderDto>(projectFrom);

        public static List<GetallimplementationCustomerGetallimplementationOrderDto> MapToGetallimplementationCustomerGetallimplementationOrderDtoList(this IEnumerable<GetAllImplementationOrder> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToGetallimplementationCustomerGetallimplementationOrderDto(mapper)).ToList();
    }
}