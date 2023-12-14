using System;
using System.Collections.Generic;
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

namespace CosmosDB.Application.Orders.GetOrderOrderItems
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderOrderItemsQueryHandler : IRequestHandler<GetOrderOrderItemsQuery, List<OrderOrderItemDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOrderOrderItemsQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OrderOrderItemDto>> Handle(
            GetOrderOrderItemsQuery request,
            CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync((request.OrderId, request.WarehouseId), cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find OrderItem '{(request.OrderId, request.WarehouseId)}'");
            }

            var orderItems = order.OrderItems;
            return orderItems.MapToOrderOrderItemDtoList(_mapper);
        }
    }
}