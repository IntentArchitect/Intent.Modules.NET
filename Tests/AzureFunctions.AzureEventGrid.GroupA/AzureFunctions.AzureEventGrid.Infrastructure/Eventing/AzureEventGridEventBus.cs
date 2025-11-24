using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridEventBus", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing
{
    public class AzureEventGridEventBus : IEventBus
    {
        private readonly AzureEventGridBehaviors.AzureEventGridPublisherPipeline _pipeline;
        private readonly List<MessageEntry> _messageQueue = [];
        private readonly Dictionary<string, AzureEventGridPublisherEntry> _lookup;

        public AzureEventGridEventBus(IOptions<AzureEventGridPublisherOptions> options,
            AzureEventGridBehaviors.AzureEventGridPublisherPipeline pipeline)
        {
            _pipeline = pipeline;
            _lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!);
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _messageQueue.Add(new MessageEntry(message, null));
        }

        public void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            _messageQueue.Add(new MessageEntry(message, additionalData));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _messageQueue.Add(new MessageEntry(message, null));
        }

        public void Send<TMessageT>(TMessageT message, IDictionary<string, object> additionalData)
            where TMessageT : class
        {
            _messageQueue.Add(new MessageEntry(message, additionalData));
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_messageQueue.Count == 0)
            {
                return;
            }

            var groupedMessages = _messageQueue.GroupBy(entry =>
            {
                var publisherEntry = _lookup[entry.Message.GetType().FullName!];
                return (publisherEntry.Endpoint, publisherEntry.CredentialKey);
            });

            foreach (var group in groupedMessages)
            {
                var (endpoint, credentialKey) = group.Key;
                var client = new EventGridPublisherClient(new Uri(endpoint), new AzureKeyCredential(credentialKey));

                var cloudEvents = new List<CloudEvent>();

                foreach (var entry in group)
                {
                    var publisherEntry = _lookup[entry.Message.GetType().FullName!];
                    var cloudEvent = CreateCloudEvent(entry, publisherEntry);

                    // Run through the pipeline individually
                    var processedEvent = await _pipeline.ExecuteAsync(cloudEvent, (@event, token) =>
                    {
                        return Task.FromResult(@event);
                    }, cancellationToken);

                    cloudEvents.Add(processedEvent);
                }
                await client.SendEventsAsync(cloudEvents, cancellationToken);
            }
            _messageQueue.Clear();
        }

        private static CloudEvent CreateCloudEvent(MessageEntry messageEntry, AzureEventGridPublisherEntry publisherEntry)
        {
            var cloudEvent = new CloudEvent(source: publisherEntry.Source, type: messageEntry.Message.GetType().FullName!, jsonSerializableData: messageEntry.Message);
            if (messageEntry.AdditionalData is not null)
            {
                if (messageEntry.AdditionalData.TryGetValue("subject", out var subject))
                {
                    cloudEvent.Subject = (string)subject;
                }

                foreach (var extensionAttribute in messageEntry.AdditionalData.Where(p => p.Key != "subject"))
                {
                    cloudEvent.ExtensionAttributes.Add(extensionAttribute.Key, extensionAttribute.Value);
                }
            }

            return cloudEvent;
        }

        private record MessageEntry(object Message, IDictionary<string, object>? AdditionalData);

    }
}