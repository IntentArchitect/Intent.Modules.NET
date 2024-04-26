using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Domain.Common.Exceptions;
using OutputCachingRedis.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Orders.GetOrderById
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
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }
            return order.MapToOrderDto(_mapper);
        }
    }
}