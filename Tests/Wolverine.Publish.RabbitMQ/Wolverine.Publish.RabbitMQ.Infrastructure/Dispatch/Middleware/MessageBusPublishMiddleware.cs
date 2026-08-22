using System.Threading;
using System.Threading.Tasks;
using ContractsMessageBus = Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;

namespace Wolverine.Publish.RabbitMQ.Infrastructure.Dispatch.Middleware
{
    /// <summary>
    /// Flushes queued Integration Events and Integration Commands to the transport once a command
    /// handler has completed. Golden sample for an artefact the Wolverine eventing module must emit
    /// into this Dispatch pipeline whenever the application publishes or sends anything.
    /// </summary>
    /// <remarks>
    /// This exists because nothing else supplies it. Under MediatR the flush came from
    /// <c>Intent.Application.MediatR.Behaviours</c>'s <c>MessageBusPublishBehaviour</c>, a pipeline
    /// behaviour owned by the DISPATCHER module. <c>Intent.Application.Wolverine</c> ships seven
    /// pipeline templates - Authorization, Validation, Logging, Performance, UnhandledException,
    /// UnitOfWork, and the ApplicationHandlerPolicy that registers them - and none of them touches
    /// the Eventing Contracts bus. So swapping MediatR for Wolverine CQRS silently removes the
    /// flush: application code calls Publish/Send, nothing throws, and the message never leaves.
    ///
    /// It runs After rather than wrapping the handler, and that ordering is the point: the flush
    /// must happen once the handler has queued its messages, and after UnitOfWorkMiddleware has
    /// committed, so nothing is dispatched on behalf of a transaction that rolled back. Wolverine
    /// skips After on a handler that threw, which is what gives that guarantee here.
    ///
    /// Registered from WolverineEventingConfiguration rather than from ApplicationHandlerPolicy,
    /// because that policy is owned and regenerated wholesale by Intent.Application.Wolverine.
    /// </remarks>
    public class MessageBusPublishMiddleware
    {
        public Task AfterAsync(ContractsMessageBus messageBus, CancellationToken cancellationToken)
        {
            return messageBus.FlushAllAsync(cancellationToken);
        }
    }
}
