using AspNetCore.AzureServiceBus.GroupB.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.MessageBusPublishBehaviour", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupB.Application.Common.Behaviours
{
    public class MessageBusPublishBehaviour<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEventBus _eventBus;

        public MessageBusPublishBehaviour(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            MediatR.RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken);

            await _eventBus.FlushAllAsync(cancellationToken);

            return response;
        }
    }
}