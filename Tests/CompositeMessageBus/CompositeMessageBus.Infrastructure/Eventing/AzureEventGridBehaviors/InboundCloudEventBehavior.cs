using Azure.Messaging;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.InboundCloudEventBehavior", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing.AzureEventGridBehaviors
{
    public class InboundCloudEventBehavior : IAzureEventGridConsumerBehavior
    {
        private readonly EventContext _eventContext;

        public InboundCloudEventBehavior(EventContext eventContext)
        {
            _eventContext = eventContext;
        }

        public async Task<CloudEvent> HandleAsync(
            CloudEvent cloudEvent,
            CloudEventBehaviorDelegate next,
            CancellationToken cancellationToken = default)
        {
            foreach (var extensionAttribute in cloudEvent.ExtensionAttributes)
            {
                _eventContext.AdditionalData.Add(extensionAttribute.Key, extensionAttribute.Value);
            }
            return await next(cloudEvent, cancellationToken);
        }
    }
}