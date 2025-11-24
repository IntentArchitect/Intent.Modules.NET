using System.Text.Json;
using CleanArchitecture.QueueStorage.Application.Common.Eventing;
using CleanArchitecture.QueueStorage.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageSubscriptionOptions", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure.Configuration
{

    public class AzureQueueStorageSubscriptionOptions
    {
        private readonly List<AzureQueueStorageSubscriptionEntry> _entries = new List<AzureQueueStorageSubscriptionEntry>();

        public IReadOnlyList<AzureQueueStorageSubscriptionEntry> Entries => _entries;

        public void Add<TMessage, THandler>(string queueName)
           where TMessage : class
           where THandler : IIntegrationEventHandler<TMessage>
        {
            ArgumentNullException.ThrowIfNull(queueName);
            _entries.Add(new AzureQueueStorageSubscriptionEntry(typeof(TMessage), AzureQueueStorageEventDispatcher.InvokeDispatchHandler<TMessage, THandler>, queueName));
        }
    }

    public delegate Task AzureQueueStorageDispatchHandler(IServiceProvider serviceProvider, AzureQueueStorageEnvelope message, JsonSerializerOptions serializationOptions, CancellationToken cancellationToken);

    public record AzureQueueStorageSubscriptionEntry(Type MessageType, AzureQueueStorageDispatchHandler HandlerAsync, string QueueName);
}