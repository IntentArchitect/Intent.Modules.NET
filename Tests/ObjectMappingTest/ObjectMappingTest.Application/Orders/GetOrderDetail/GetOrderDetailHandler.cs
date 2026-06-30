using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMappingTest.Domain.Common.Exceptions;
using ObjectMappingTest.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders.GetOrderDetail
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderDetailHandler : IRequestHandler<GetOrderDetail, OrderDetailDto>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public GetOrderDetailHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<OrderDetailDto> Handle(GetOrderDetail request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (order is null) throw new NotFoundException($"Could not find Order '{request.Id}'");
            return order.MapToOrderDetailDto();
        }
    }
}