using Azure.Messaging;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridMessageDispatcherInterface", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public interface IAzureEventGridMessageDispatcher
    {
        Task DispatchAsync(IServiceProvider scopedServiceProvider, CloudEvent cloudEvent, CancellationToken cancellationToken);
    }
}