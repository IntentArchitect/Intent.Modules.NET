using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing.Behaviors;

public interface IAzureEventGridPublisherBehavior
{
    Task<CloudEvent> HandleAsync(CloudEvent cloudEvent, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default);
}

public interface IAzureEventGridConsumerBehavior
{
    Task<CloudEvent> HandleAsync(CloudEvent cloudEvent, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default);
}

public delegate Task<CloudEvent> CloudEventBehaviorDelegate(CloudEvent cloudEvent, CancellationToken cancellationToken);