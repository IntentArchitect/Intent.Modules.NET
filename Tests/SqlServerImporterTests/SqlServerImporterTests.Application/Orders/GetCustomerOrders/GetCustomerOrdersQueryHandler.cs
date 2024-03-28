using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SqlServerImporterTests.Domain.Repositories.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace SqlServerImporterTests.Application.Orders.GetCustomerOrders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, List<CustomerOrderDto>>
    {
        private readonly IOrderRepository2Repository _orderRepository2Repository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomerOrdersQueryHandler(IOrderRepository2Repository orderRepository2Repository, IMapper mapper)
        {
            _orderRepository2Repository = orderRepository2Repository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerOrderDto>> Handle(
            GetCustomerOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _orderRepository2Repository.GetCustomerOrders(request.CustomerID, cancellationToken);
            return result.MapToCustomerOrderDtoList(_mapper);
        }


    }
}