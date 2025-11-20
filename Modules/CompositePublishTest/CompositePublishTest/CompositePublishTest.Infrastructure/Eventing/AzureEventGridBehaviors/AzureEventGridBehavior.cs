using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridBehavior", Version = "1.0")]

namespace CompositePublishTest.Infrastructure.Eventing.AzureEventGridBehaviors;

public interface IAzureEventGridPublisherBehavior
{
    Task<CloudEvent> HandleAsync(CloudEvent cloudEvent, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default);
}

public interface IAzureEventGridConsumerBehavior
{
    Task<CloudEvent> HandleAsync(CloudEvent cloudEvent, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default);
}

public delegate Task<CloudEvent> CloudEventBehaviorDelegate(CloudEvent cloudEvent, CancellationToken cancellationToken);