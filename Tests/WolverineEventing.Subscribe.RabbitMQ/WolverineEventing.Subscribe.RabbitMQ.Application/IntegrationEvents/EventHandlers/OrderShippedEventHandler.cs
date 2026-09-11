using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using WolverineEventing.Publish.RabbitMQ.Eventing.Messages;
using WolverineEventing.Subscribe.RabbitMQ.Application.Common.Eventing;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Entities;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Repositories;
using WolverineEventing.Subscribe.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderShippedEventHandler : IIntegrationEventHandler<OrderShippedEvent>
    {
        private readonly IMessageBus _messageBus;
        private readonly IShippedOrderRecordRepository _shippedOrderRecordRepository;
        private readonly ILogger<OrderShippedEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public OrderShippedEventHandler(ILogger<OrderShippedEventHandler> logger, IMessageBus messageBus, IShippedOrderRecordRepository shippedOrderRecordRepository)
        {
            _logger = logger;
            _messageBus = messageBus;
            _shippedOrderRecordRepository = shippedOrderRecordRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(OrderShippedEvent message, CancellationToken cancellationToken = default)
        {
            var shippedOrderRecord = new ShippedOrderRecord
            {
                OrderId = message.OrderId,
                ShippedAt = message.ShippedAt
            };
            _shippedOrderRecordRepository.Add(shippedOrderRecord);
            _messageBus.Publish(new OrderShipmentRecordedEvent
            {
                OrderId = message.OrderId
            });
        }
    }
}
