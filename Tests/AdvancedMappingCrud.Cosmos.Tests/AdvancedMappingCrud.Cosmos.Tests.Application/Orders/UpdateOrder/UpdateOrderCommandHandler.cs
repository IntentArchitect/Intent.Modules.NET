using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.UpdateOrder
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

            order.CustomerId = request.CustomerId;
            order.RefNo = request.RefNo;
            order.OrderDate = request.OrderDate;
            order.OrderStatus = request.OrderStatus;
            order.OrderTags = UpdateHelper.CreateOrUpdateCollection(order.OrderTags, request.OrderTags, (e, d) => e.Equals(new OrderTags(
                name: d.Name,
                value: d.Value)), CreateOrUpdateOrderTags);

            _orderRepository.Update(order);
        }

        [IntentManaged(Mode.Fully)]
        private static OrderTags CreateOrUpdateOrderTags(OrderTags? valueObject, UpdateOrderCommandOrderTagsDto dto)
        {
            if (valueObject is null)
            {
                return new OrderTags(
                    name: dto.Name,
                    value: dto.Value);
            }
            return valueObject;
        }
    }
}