using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridEventBus", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing
{
    public class AzureEventGridEventBus : IEventBus
    {
        private readonly List<MessageEntry> _messageQueue = [];
        private readonly Dictionary<string, PublisherEntry> _lookup;

        public AzureEventGridEventBus(IOptions<PublisherOptions> options)
        {
            _lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!);
        }

        public void Publish<T>(T message)
            where T : class
        {
            ValidateMessage(message);
            _messageQueue.Add(new MessageEntry(message, null, null));
        }

        public void Publish<T>(T message, string subject, IDictionary<string, object>? extensionAttributes = null)
            where T : class
        {
            ValidateMessage(message);
            _messageQueue.Add(new MessageEntry(message, subject, extensionAttributes));
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_messageQueue.Count == 0)
            {
                return;
            }

            foreach (var entry in _messageQueue)
            {
                var publisherEntry = _lookup[entry.Message.GetType().FullName!];
                var client = new EventGridPublisherClient(new Uri(publisherEntry.Endpoint), new AzureKeyCredential(publisherEntry.CredentialKey));
                var eventGridEvent = CreateCloudEvent(entry, publisherEntry);
                await client.SendEventAsync(eventGridEvent, cancellationToken);
            }
        }

        private void ValidateMessage(object message)
        {
            if (!_lookup.TryGetValue(message.GetType().FullName!, out _))
            {
                throw new Exception($"The message type '{message.GetType().FullName}' is not registered.");
            }
        }

        private static CloudEvent CreateCloudEvent(MessageEntry messageEntry, PublisherEntry publisherEntry)
        {
            var cloudEvent = new CloudEvent(source: publisherEntry.Source, type: messageEntry.Message.GetType().FullName!, jsonSerializableData: messageEntry.Message)
            {
                Subject = messageEntry.Subject,
            };
            if (messageEntry.ExtensionAttributes is not null)
            {
                foreach (var extensionAttribute in messageEntry.ExtensionAttributes)
                {
                    cloudEvent.ExtensionAttributes.Add(extensionAttribute.Key, extensionAttribute.Value);
                }
            }

            return cloudEvent;
        }

        private record MessageEntry(object Message, string? Subject, IDictionary<string, object>? ExtensionAttributes);

    }
}