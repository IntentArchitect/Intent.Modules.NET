using System.Threading;
using System.Threading.Tasks;
using Wolverine.Publish.RabbitMQ.Eventing.Messages;
using Wolverine.Subscribe.RabbitMQ.Application.Common.Eventing;

namespace Wolverine.Subscribe.RabbitMQ.Infrastructure.Eventing
{
    public class ProcessOrderCommandConsumer
    {
        private readonly IIntegrationEventHandler<ProcessOrderCommand> _handler;

        public ProcessOrderCommandConsumer(IIntegrationEventHandler<ProcessOrderCommand> handler)
        {
            _handler = handler;
        }

        public Task HandleAsync(ProcessOrderCommand message, CancellationToken cancellationToken)
        {
            return _handler.HandleAsync(message, cancellationToken);
        }
    }
}
