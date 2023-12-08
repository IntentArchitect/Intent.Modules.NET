using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.GetOrderOrderItemById
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
            var order = await _orderRepository.FindByIdAsync(request.OrderId, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find OrderItem '{request.OrderId}'");
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