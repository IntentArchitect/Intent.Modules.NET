using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Messaging.ServiceBus;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WindowsServiceHost.Tests.Common.Eventing;
using WindowsServiceHost.Tests.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusEventBus", Version = "1.0")]

namespace WindowsServiceHost.Tests.Eventing
{
    public class AzureServiceBusEventBus : IEventBus
    {
        private readonly List<object> _messageQueue = [];
        private readonly Dictionary<string, string> _lookup;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusClient _serviceBusClient;

        public AzureServiceBusEventBus(IConfiguration configuration,
            ServiceBusClient serviceBusClient,
            IOptions<AzureServiceBusPublisherOptions> options)
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