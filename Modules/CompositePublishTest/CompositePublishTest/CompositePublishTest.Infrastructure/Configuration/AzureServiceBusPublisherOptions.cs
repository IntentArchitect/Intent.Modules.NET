using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CompositePublishTest.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration options for Azure Service Bus message publishing.
    /// Maps message types to their destination queues or topics.
    /// </summary>
    public class AzureServiceBusPublisherOptions
    {
        private readonly List<ServiceBusPublisherEntry> _entries = [];

        public IReadOnlyList<ServiceBusPublisherEntry> Entries => _entries;

        /// <summary>
        /// Register a message type to be published to a specific queue or topic.
        /// </summary>
        public void Add<TMessage>(string queueOrTopicName)
        {
            ArgumentNullException.ThrowIfNull(queueOrTopicName);
            _entries.Add(new ServiceBusPublisherEntry(typeof(TMessage), queueOrTopicName));
        }
    }

    /// <summary>
    /// Represents a Service Bus publisher entry (message type to queue/topic mapping).
    /// </summary>
    public record ServiceBusPublisherEntry(Type MessageType, string QueueOrTopicName);
}
