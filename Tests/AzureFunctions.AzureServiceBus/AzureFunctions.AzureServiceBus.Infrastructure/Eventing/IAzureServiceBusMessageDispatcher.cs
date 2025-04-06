using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusMessageDispatcherInterface", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Infrastructure.Eventing
{
    public interface IAzureServiceBusMessageDispatcher
    {
        Task DispatchAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken);
    }
}