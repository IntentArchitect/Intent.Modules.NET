using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders.GetOrdersFiltered
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrdersFilteredQueryHandler : IRequestHandler<GetOrdersFilteredQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOrdersFilteredQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OrderDto>> Handle(GetOrdersFilteredQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.FindAllAsync((p) => p.PartitionKey == request.PartitionKey, cancellationToken);
            return orders.MapToOrderDtoList(_mapper);
        }
    }
}