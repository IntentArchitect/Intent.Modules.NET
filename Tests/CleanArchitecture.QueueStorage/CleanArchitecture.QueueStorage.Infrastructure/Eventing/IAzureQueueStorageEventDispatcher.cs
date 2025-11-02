using System.Text.Json;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageEventDispatcherInterface", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure.Eventing
{
    public interface IAzureQueueStorageEventDispatcher
    {
        Task DispatchAsync(IServiceProvider serviceProvider, AzureQueueStorageEnvelope message, JsonSerializerOptions serializerOptions, CancellationToken cancellationToken);
    }
}