using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.PatchOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PatchOrderCommandHandler : IRequestHandler<PatchOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public PatchOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(PatchOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }


            if (request.RefNo is not null)
            {
                order.RefNo = request.RefNo;
            }

            if (request.OrderDate is not null)
            {
                order.OrderDate = request.OrderDate.Value;
            }

            if (request.OrderStatus is not null)
            {
                order.OrderStatus = request.OrderStatus.Value;
            }

            if (request.CustomerId is not null)
            {
                order.CustomerId = request.CustomerId.Value;
            }

            if (request.DeliveryAddress is not null)
            {
                order.DeliveryAddress = new Address(
                    line1: request.DeliveryAddress.Line1,
                    line2: request.DeliveryAddress?.Line2 ?? order.DeliveryAddress.Line2,
                    city: request.DeliveryAddress?.City ?? order.DeliveryAddress.City,
                    postal: request.DeliveryAddress?.Postal ?? order.DeliveryAddress.Postal);
            }

            if (request.BillingAddress is not null)
            {
                order.BillingAddress = request.BillingAddress is not null
                    ? new Address(
                        line1: request.BillingAddress.Line1,
                        line2: request.BillingAddress?.Line2 ?? order.BillingAddress?.Line2,
                        city: request.BillingAddress?.City ?? order.BillingAddress?.City,
                        postal: request.BillingAddress?.Postal ?? order.BillingAddress?.Postal)
                    : null;
            }
        }
    }
}