using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.EventBusPublishBehaviour", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.Common.Behaviours;

public class EventBusPublishBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull
{
    private readonly IEventBus _eventBus;

    public EventBusPublishBehaviour(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        await _eventBus.FlushAllAsync(cancellationToken);

        return response;
    }
}