using System.Threading;
using System.Threading.Tasks;
using Wolverine.Publish.RabbitMQ.Eventing.Messages;
using Wolverine.Subscribe.RabbitMQ.Application.Common.Eventing;

namespace Wolverine.Subscribe.RabbitMQ.Infrastructure.Eventing
{
    public class OrderShippedEventConsumer
    {
        private readonly IIntegrationEventHandler<OrderShippedEvent> _handler;

        public OrderShippedEventConsumer(IIntegrationEventHandler<OrderShippedEvent> handler)
        {
            _handler = handler;
        }

        public Task HandleAsync(OrderShippedEvent message, CancellationToken cancellationToken)
        {
            return _handler.HandleAsync(message, cancellationToken);
        }
    }
}
