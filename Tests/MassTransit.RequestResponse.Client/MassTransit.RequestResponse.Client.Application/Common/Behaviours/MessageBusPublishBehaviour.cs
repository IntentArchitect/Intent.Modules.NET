using Intent.RoslynWeaver.Attributes;
using MassTransit.RequestResponse.Client.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.MessageBusPublishBehaviour", Version = "1.0")]

namespace MassTransit.RequestResponse.Client.Application.Common.Behaviours
{
    public class MessageBusPublishBehaviour<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IMessageBus _messageBus;

        public MessageBusPublishBehaviour(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            MediatR.RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken);

            await _messageBus.FlushAllAsync(cancellationToken);

            return response;
        }
    }
}