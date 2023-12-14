using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Common.Exceptions;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Orders.GetOrderById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync((request.Id, request.WarehouseId), cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '({request.Id}, {request.WarehouseId})'");
            }
            return order.MapToOrderDto(_mapper);
        }
    }
}