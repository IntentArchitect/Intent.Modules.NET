using CqrsSpike.Publisher.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.Behaviours.MessageBusPublishBehaviour", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.Common.Behaviours
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