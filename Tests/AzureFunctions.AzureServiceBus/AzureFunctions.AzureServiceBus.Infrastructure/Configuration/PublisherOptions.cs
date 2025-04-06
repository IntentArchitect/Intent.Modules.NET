using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.PublisherOptions", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Infrastructure.Configuration
{
    public class PublisherOptions
    {
        private readonly List<PublisherEntry> _entries = [];

        public IReadOnlyList<PublisherEntry> Entries => _entries;

        public void Add<TMessage>(string queueOrTopicName)
        {
            ArgumentNullException.ThrowIfNull(queueOrTopicName);
            _entries.Add(new PublisherEntry(typeof(TMessage), queueOrTopicName));
        }
    }

    public record PublisherEntry(Type MessageType, string QueueOrTopicName);
}