using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CompositePublishTest.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration options for MassTransit message publishing.
    /// Maps message types to their destination addresses or configures for standard publish.
    /// </summary>
    public class MassTransitPublisherOptions
    {
        private readonly List<MassTransitPublisherEntry> _entries = [];

        public IReadOnlyList<MassTransitPublisherEntry> Entries => _entries;

        /// <summary>
        /// Register a message type to be published via MassTransit publish semantics (pub/sub).
        /// </summary>
        public void AddPublish<TMessage>()
        {
            _entries.Add(new MassTransitPublisherEntry(typeof(TMessage), null));
        }

        /// <summary>
        /// Register a message type to be sent to a specific address via MassTransit send semantics.
        /// </summary>
        public void AddSend<TMessage>(string address)
        {
            ArgumentNullException.ThrowIfNull(address);
            _entries.Add(new MassTransitPublisherEntry(typeof(TMessage), address));
        }
    }

    /// <summary>
    /// Represents a MassTransit publisher entry (message type to publish/send routing).
    /// Stores whether the message should be published (pub/sub) or sent to a specific address.
    /// </summary>
    public class MassTransitPublisherEntry
    {
        public MassTransitPublisherEntry(Type messageType, string? address)
        {
            MessageType = messageType;
            Address = address;
        }

        public Type MessageType { get; set; }
        
        /// <summary>
        /// The destination address for Send semantics. Null means use Publish semantics.
        /// </summary>
        public string? Address { get; set; }
    }
}
