using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusPublisherOptions", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupA.Infrastructure.Configuration
{
    public class AzureServiceBusPublisherOptions
    {
        private readonly List<AzureServiceBusPublisherEntry> _entries = [];

        public IReadOnlyList<AzureServiceBusPublisherEntry> Entries => _entries;

        public void Add<TMessage>(string queueOrTopicName)
        {
            ArgumentNullException.ThrowIfNull(queueOrTopicName);
            _entries.Add(new AzureServiceBusPublisherEntry(typeof(TMessage), queueOrTopicName));
        }
    }

    public record AzureServiceBusPublisherEntry(Type MessageType, string QueueOrTopicName);
}