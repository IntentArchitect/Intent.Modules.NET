using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Messaging.ServiceBus;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusEventBus", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Infrastructure.Eventing
{
    public class AzureServiceBusEventBus : IEventBus
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly List<MessageEntry> _messageQueue = [];
        private readonly Dictionary<string, string> _lookup;

        public AzureServiceBusEventBus(ServiceBusClient serviceBusClient,
            IOptions<AzureServiceBusPublisherOptions> options)
        {
            _serviceBusClient = serviceBusClient;
            _lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.QueueOrTopicName);
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

        public void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            _messageQueue.Add(new MessageEntry(message, additionalData));
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
                var queueOrTopicName = _lookup[message.Message.GetType().FullName!];
                await using var sender = _serviceBusClient.CreateSender(queueOrTopicName);
                var serviceBusMessage = CreateServiceBusMessage(message);
                await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
            }
            scope.Complete();
            _messageQueue.Clear();
        }

        private static ServiceBusMessage CreateServiceBusMessage(MessageEntry message)
        {
            var serializedMessage = JsonSerializer.Serialize(message.Message);
            var serviceBusMessage = new ServiceBusMessage(serializedMessage);
            serviceBusMessage.ApplicationProperties["MessageType"] = message.Message.GetType().FullName;
            if (message.AdditionalData != null)
            {
                foreach (var dataEnty in message.AdditionalData)
                {
                    serviceBusMessage.ApplicationProperties[dataEnty.Key] = dataEnty.Value;
                }
            }
            return serviceBusMessage;
        }

        private record MessageEntry(object Message, IDictionary<string, object>? AdditionalData);
    }
}