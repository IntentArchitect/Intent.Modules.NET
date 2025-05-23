using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridMessageDispatcherInterface", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing
{
    public interface IAzureEventGridMessageDispatcher
    {
        Task DispatchAsync(IServiceProvider scopedServiceProvider, EventGridEvent message, CancellationToken cancellationToken);
    }
}