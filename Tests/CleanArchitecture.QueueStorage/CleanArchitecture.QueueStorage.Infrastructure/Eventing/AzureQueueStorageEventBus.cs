using System.Text.Json;
using Azure.Storage.Queues;
using CleanArchitecture.QueueStorage.Application.Common.Eventing;
using CleanArchitecture.QueueStorage.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageEventBus", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure.Eventing
{
    public class AzureQueueStorageEventBus : IEventBus
    {
        private readonly List<AzureQueueStorageEnvelope> _messageQueue = new List<AzureQueueStorageEnvelope>();
        private readonly Dictionary<string, QueueDefinition> _lookup;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<AzureQueueStorageEventBus> _logger;
        private readonly IOptions<AzureQueueStorageOptions> _options;

        public AzureQueueStorageEventBus(ILogger<AzureQueueStorageEventBus> logger,
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

        public void Publish<T>(T message)
            where T : class
        {
            ValidateMessage(message);
            _messageQueue.Add(new AzureQueueStorageEnvelope(message));
        }

        public void Send<T>(T message)
            where T : class
        {
            ValidateMessage(message);
            _messageQueue.Add(new AzureQueueStorageEnvelope(message));
        }

        public void ValidateMessage(object message)
        {
            if (!_lookup.TryGetValue(message.GetType().FullName!, out _))
            {
                throw new Exception($"The message type '{message.GetType().FullName}' is not registered.");
            }
        }
    }
}