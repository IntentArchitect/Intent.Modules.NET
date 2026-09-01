using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using WolverineEventing.Coexist.Cqrs.Application.Common.Eventing;
using WolverineEventing.Coexist.Cqrs.Domain.Entities;
using WolverineEventing.Coexist.Cqrs.Domain.Repositories;
using WolverineEventing.Coexist.Cqrs.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace WolverineEventing.Coexist.Cqrs.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            // No explicit SaveChangesAsync call: under Transactional Outbox = Durable, Wolverine's
            // AutoApplyTransactions() already wraps this handler in a transaction and saves via
            // whatever DbContext its resolved dependencies touch - verified empirically (see
            // Intent.Eventing.Wolverine's CONTEXT.md). Matches the convention Command/Query handlers
            // already follow: the middleware saves, the handler body does not.
            _orderRepository.Add(new Order { Id = message.OrderId, Status = "Created" });
            _logger.LogInformation("HANDLED OrderCreatedEvent OrderId={OrderId}", message.OrderId);
        }
    }
}
