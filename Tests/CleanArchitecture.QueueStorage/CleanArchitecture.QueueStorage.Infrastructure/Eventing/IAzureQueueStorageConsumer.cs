using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageConsumerInterface", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure.Eventing
{
    public interface IAzureQueueStorageConsumer
    {
        Task ConsumeAsync(CancellationToken cancellationToken);
    }
}