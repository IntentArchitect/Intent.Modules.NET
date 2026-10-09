using CqrsSpike.Subscriber.Application.Common.Eventing;
using Mediator;

namespace CqrsSpike.Subscriber.Application.Common.Behaviours
{
    public class MessageBusPublishBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IMessage
    {
        private readonly IMessageBus _messageBus;

        public MessageBusPublishBehaviour(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public async ValueTask<TResponse> Handle(
            TRequest request,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next(request, cancellationToken);

            await _messageBus.FlushAllAsync(cancellationToken);

            return response;
        }
    }
}