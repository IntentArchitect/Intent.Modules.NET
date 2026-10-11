using CqrsSpike.Subscriber.Application.Common.Eventing;
using Mediator;

namespace CqrsSpike.Subscriber.Application.Common.Behaviours
{
    public class MessageBusPublishBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IMessage
    {
        private readonly IMessageBus _messageBus;
        private readonly MessageBusFlushOwner _flushOwner;

        public MessageBusPublishBehaviour(IMessageBus messageBus, MessageBusFlushOwner flushOwner)
        {
            _messageBus = messageBus;
            _flushOwner = flushOwner;
        }

        public async ValueTask<TResponse> Handle(
            TRequest request,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            var isOwner = _flushOwner.Enter();
            try
            {
                var response = await next(request, cancellationToken);

                if (isOwner)
                {
                    await _messageBus.FlushAllAsync(cancellationToken);
                }

                return response;
            }
            finally
            {
                _flushOwner.Exit();
            }
        }
    }
}