using System.Threading;
using System.Threading.Tasks;
using ContractsMessageBus = WolverineEventing.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;

namespace WolverineEventing.Publish.RabbitMQ.Infrastructure.Dispatch.Middleware
{
    /// <summary>
    /// Flushes queued Integration Events and Integration Commands to the transport once a command
    /// handler has completed. Golden sample for an artefact the Wolverine eventing module must emit
    /// into this Dispatch pipeline whenever the application publishes or sends anything.
    /// </summary>
    /// <remarks>
    /// Runs After rather than wrapping the handler, and that ordering is the point: the flush must
    /// happen once the handler has queued its messages, and after UnitOfWorkMiddleware has
    /// committed, so nothing is dispatched for a transaction that rolled back. Wolverine skips
    /// After on a handler that threw, which is what gives that guarantee.
    /// </remarks>
    public class MessageBusPublishMiddleware
    {
        public Task AfterAsync(ContractsMessageBus messageBus, CancellationToken cancellationToken)
        {
            return messageBus.FlushAllAsync(cancellationToken);
        }
    }
}
