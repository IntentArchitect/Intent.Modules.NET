using AutoMapper;
using CqrsSpike.Publisher.Domain.Common.Exceptions;
using CqrsSpike.Publisher.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryHandler", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.Orders.GetOrderById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
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
        public async ValueTask<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
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