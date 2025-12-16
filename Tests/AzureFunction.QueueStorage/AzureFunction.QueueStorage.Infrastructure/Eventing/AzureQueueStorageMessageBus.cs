using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using AzureFunction.QueueStorage.Application.Common.Eventing;
using AzureFunction.QueueStorage.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageMessageBus", Version = "1.0")]

namespace AzureFunction.QueueStorage.Infrastructure.Eventing
{
    public class AzureQueueStorageMessageBus : IEventBus
    {
        private readonly List<AzureQueueStorageEnvelope> _messageQueue = new List<AzureQueueStorageEnvelope>();
        private readonly Dictionary<string, QueueDefinition> _lookup;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<AzureQueueStorageMessageBus> _logger;
        private readonly IOptions<AzureQueueStorageOptions> _options;

        public AzureQueueStorageMessageBus(ILogger<AzureQueueStorageMessageBus> logger,
            IOptions<AzureQueueStorageOptions> options)
        {
            _logger = logger;
            _options = options;
            _lookup = options.Value.QueueTypeMap.Select(
                t =>
                {
                    if (!options.Value.Queues.TryGetValue(t.Value, out QueueDefinition? value))
                    {
                        throw new ArgumentNullException($"No queue name '{t.Value}' found for type '{t.Key}'");
                    }
                    value.QueueName = t.Value;
                    return new KeyValuePair<string, QueueDefinition>(t.Key, value);
                }).ToDictionary();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_messageQueue.Count == 0)
            {
                return;
            }
            var groupedMessages = _messageQueue.GroupBy(entry =>
            {
                var publisherEntry = _lookup[entry.MessageType!];
                publisherEntry.Endpoint = !string.IsNullOrWhiteSpace(publisherEntry.Endpoint) ? publisherEntry.Endpoint : _options.Value.DefaultEndpoint;
                return (publisherEntry.Endpoint, publisherEntry.QueueName, publisherEntry.CreateQueue);
            });

            foreach (var group in groupedMessages)
            {
                var queueName = group.Key.QueueName;
                var endpoint = group.Key.Endpoint;
                var createQueue = group.Key.CreateQueue;
                var queueClient = new QueueClient(endpoint, queueName);

                if (createQueue)
                {
                    await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                }

                if (!await queueClient.ExistsAsync(cancellationToken))
                {
                    _logger.LogError("Queue '{QueueName}' does not exist. Unable to publish.", queueName);
                    continue;
                }

                foreach (var entry in group)
                {
                    var payload = JsonSerializer.Serialize(entry, _serializerOptions);
                    await queueClient.SendMessageAsync(payload, cancellationToken);
                }
            }
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _messageQueue.Add(new AzureQueueStorageEnvelope(message));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _messageQueue.Add(new AzureQueueStorageEnvelope(message));
        }
    }
}