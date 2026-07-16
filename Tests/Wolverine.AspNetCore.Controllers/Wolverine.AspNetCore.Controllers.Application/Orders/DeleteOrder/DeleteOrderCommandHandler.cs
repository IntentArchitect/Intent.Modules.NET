using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Common.Exceptions;
using Wolverine.AspNetCore.Controllers.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.DeleteOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOrderCommandHandler
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
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }


            _orderRepository.Remove(order);
        }
    }
}