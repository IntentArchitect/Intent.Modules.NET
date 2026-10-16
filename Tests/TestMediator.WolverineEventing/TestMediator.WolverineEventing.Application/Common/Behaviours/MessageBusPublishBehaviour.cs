using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.WolverineEventing.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.MessageBusPublishBehaviour", Version = "1.0")]

namespace TestMediator.WolverineEventing.Application.Common.Behaviours
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