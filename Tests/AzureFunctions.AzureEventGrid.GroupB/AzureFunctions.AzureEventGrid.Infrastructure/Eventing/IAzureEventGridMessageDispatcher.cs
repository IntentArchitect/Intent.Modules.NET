using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridMessageDispatcherInterface", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing
{
    public interface IAzureEventGridMessageDispatcher
    {
        Task DispatchAsync(IServiceProvider scopedServiceProvider, CloudEvent cloudEvent, CancellationToken cancellationToken);
    }
}