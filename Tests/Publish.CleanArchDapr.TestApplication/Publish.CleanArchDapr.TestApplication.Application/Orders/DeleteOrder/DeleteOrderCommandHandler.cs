using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Domain.Common.Exceptions;
using Publish.CleanArchDapr.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Orders.DeleteOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var existingOrder = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOrder is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }

            _orderRepository.Remove(existingOrder);
        }
    }
}