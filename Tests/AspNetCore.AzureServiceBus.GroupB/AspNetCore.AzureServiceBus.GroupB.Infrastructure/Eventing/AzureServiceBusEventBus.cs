using System.Text.Json;
using System.Transactions;
using AspNetCore.AzureServiceBus.GroupB.Application.Common.Eventing;
using AspNetCore.AzureServiceBus.GroupB.Infrastructure.Configuration;
using Azure.Messaging.ServiceBus;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusEventBus", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupB.Infrastructure.Eventing
{
    public class AzureServiceBusEventBus : IEventBus
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly List<object> _messageQueue = [];
        private readonly Dictionary<string, string> _lookup;

        public AzureServiceBusEventBus(IConfiguration configuration, ServiceBusClient serviceBusClient, IOptions<PublisherOptions> options)
        {
            _configuration = configuration;
            _serviceBusClient = serviceBusClient;
            _lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.QueueOrTopicName);
        }

        public void Publish<T>(T message)
            where T : class
        {
            ValidateMessage(message);
            _messageQueue.Add(message);
        }

        public void Send<T>(T message)
            where T : class
        {
            ValidateMessage(message);
            _messageQueue.Add(message);
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_messageQueue.Count == 0)
            {
                return;
            }
            using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            foreach (var message in _messageQueue)
            {
                var queueOrTopicName = _lookup[message.GetType().FullName!];
                await using var sender = _serviceBusClient.CreateSender(queueOrTopicName);
                var serviceBusMessage = CreateServiceBusMessage(message);
                await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
            }
            scope.Complete();
        }

        private void ValidateMessage(object message)
        {
            if (!_lookup.TryGetValue(message.GetType().FullName!, out _))
            {
                throw new Exception($"The message type '{message.GetType().FullName}' is not registered.");
            }
        }

        private static ServiceBusMessage CreateServiceBusMessage(object message)
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(serializedMessage);
            serviceBusMessage.ApplicationProperties["MessageType"] = message.GetType().FullName;
            return serviceBusMessage;
        }
    }
}