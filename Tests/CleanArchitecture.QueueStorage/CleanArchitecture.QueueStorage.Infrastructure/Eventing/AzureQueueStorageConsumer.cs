using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using CleanArchitecture.QueueStorage.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageConsumer", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure.Eventing
{
    public class AzureQueueStorageConsumer : IAzureQueueStorageConsumer
    {
        private readonly ILogger<AzureQueueStorageConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<AzureQueueStorageOptions> _options;
        private readonly QueueDefinition _queueDefinition;
        private readonly Dictionary<string, AzureQueueStorageDispatchHandler> _handlers;
        private readonly JsonSerializerOptions _serializerOptions;

        public AzureQueueStorageConsumer(ILogger<AzureQueueStorageConsumer> logger,
            IServiceProvider serviceProvider,
            IOptions<AzureQueueStorageOptions> options,
            IOptions<AzureQueueStorageSubscriptionOptions> subscriptionOptions,
            QueueDefinition queueDefinition)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options;
            _queueDefinition = queueDefinition;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            _handlers = subscriptionOptions.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Subscribing to Queue: {_queueDefinition.QueueName}");

            try
            {
                var endpoint = !string.IsNullOrWhiteSpace(_queueDefinition.Endpoint) ? _queueDefinition.Endpoint : _options.Value.DefaultEndpoint;
                var queueClient = new QueueClient(_queueDefinition.Endpoint, _queueDefinition.QueueName);

                if (_queueDefinition.CreateQueue)
                {
                    await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!await queueClient.ExistsAsync(cancellationToken))
                    {
                        _logger.LogError("Queue '{QueueName}' does not exist. Unable to consume.", _queueDefinition.QueueName);
                        await Task.Delay(500, cancellationToken);
                        continue;
                    }
                    QueueProperties properties = await queueClient.GetPropertiesAsync(cancellationToken);

                    if (properties.ApproximateMessagesCountLong <= 0)
                    {
                        await Task.Delay(500, cancellationToken);
                        continue;
                    }
                    QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(maxMessages: _queueDefinition.MaxMessages);

                    if (messages.Length == 0)
                    {
                        await Task.Delay(500, cancellationToken);
                        continue;
                    }

                    foreach (var message in messages)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            try
                            {
                                var envelope = message.Body.ToObjectFromJson<AzureQueueStorageEnvelope>(_serializerOptions);

                                if (envelope is null)
                                {
                                    _logger.LogWarning("Skipping message '{Id}'. Null deserialization", message.MessageId);
                                    continue;
                                }
                                var messageTypeName = envelope.MessageType;

                                if (_handlers.TryGetValue(messageTypeName, out var handlerAsync))
                                {
                                    await handlerAsync(scope.ServiceProvider, envelope, _serializerOptions, cancellationToken);
                                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
                                }
                            }
                            catch (Exception handlerEx)
                            {
                                _logger.LogError(handlerEx, "Error dispatching message '{Id}' from queue '{Queue}'", message.MessageId, _queueDefinition.QueueName);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error consuming for {_queueDefinition.QueueName}");
            }
        }
    }
}