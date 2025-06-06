using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing.Behaviors;

public class InboundCloudEventBehavior : IAzureEventGridConsumerBehavior
{
    private readonly CloudEventContext _cloudEventContext;

    public InboundCloudEventBehavior(CloudEventContext cloudEventContext)
    {
        _cloudEventContext = cloudEventContext;
    }

    public async Task<CloudEvent> HandleAsync(CloudEvent cloudEvent, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default)
    {
        foreach (var extensionAttribute in cloudEvent.ExtensionAttributes)
        {
            _cloudEventContext.ExtensionAttributes.Add(extensionAttribute.Key, extensionAttribute.Value);
        }

        return await next(cloudEvent, cancellationToken);
    }
}