using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.Orders.GetOrderOrderItemById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderOrderItemByIdQueryHandler : IRequestHandler<GetOrderOrderItemByIdQuery, OrderOrderItemDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOrderOrderItemByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OrderOrderItemDto> Handle(
            GetOrderOrderItemByIdQuery request,
            CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync((request.OrderId, request.WarehouseId), cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find OrderItem '{(request.OrderId, request.WarehouseId)}'");
            }

            var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == request.Id);
            if (orderItem is null)
            {
                throw new NotFoundException($"Could not find OrderItem '{request.Id}'");
            }
            return orderItem.MapToOrderOrderItemDto(_mapper);
        }
    }
}