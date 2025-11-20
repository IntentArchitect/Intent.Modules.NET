using System.Threading;
using System.Threading.Tasks;
using CompositePublishTest.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.EventBusPublishBehaviour", Version = "1.0")]

namespace CompositePublishTest.Application.Common.Behaviours;

public class EventBusPublishBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull
{
    private readonly IMessageBus _messageBus;

    public EventBusPublishBehaviour(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        await _messageBus.FlushAllAsync(cancellationToken);

        return response;
    }
}