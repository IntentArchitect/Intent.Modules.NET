using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageEventDispatcherInterface", Version = "1.0")]

namespace AzureFunction.QueueStorage.Infrastructure.Eventing
{
    public interface IAzureQueueStorageEventDispatcher
    {
        Task DispatchAsync(IServiceProvider serviceProvider, AzureQueueStorageEnvelope message, JsonSerializerOptions serializerOptions, CancellationToken cancellationToken);
    }
}