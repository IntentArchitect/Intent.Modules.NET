using AspNetCoreCleanArchitecture.Sample.Domain.Common;
using AspNetCoreCleanArchitecture.Sample.Domain.Common.Exceptions;
using AspNetCoreCleanArchitecture.Sample.Domain.Entities;
using AspNetCoreCleanArchitecture.Sample.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Orders.UpdateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }

            order.BuyerId = request.BuyerId;
            order.OrderLines = UpdateHelper.CreateOrUpdateCollection(order.OrderLines, request.OrderLines, (e, d) => e.Id == d.Id, CreateOrUpdateOrderLine);
        }

        [IntentManaged(Mode.Fully)]
        private static OrderLine CreateOrUpdateOrderLine(OrderLine? entity, UpdateOrderCommandOrderLinesDto dto)
        {
            entity ??= new OrderLine();
            entity.Id = dto.Id;
            entity.ProductId = dto.ProductId;
            entity.Units = dto.Units;
            entity.UnitPrice = dto.UnitPrice;
            entity.Discount = dto.Discount;
            return entity;
        }
    }
}