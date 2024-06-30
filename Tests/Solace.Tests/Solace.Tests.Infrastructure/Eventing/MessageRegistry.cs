using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Solace.Tests.Eventing.Messages;
using Solace.Tests.Infrastructure.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.MessageRegistry", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Eventing
{
    public class MessageRegistry
    {
        private readonly Dictionary<Type, string> _typeNameMap = new Dictionary<Type, string>();
        private readonly Dictionary<Type, PublishingInfo> _publishedMessages = new Dictionary<Type, PublishingInfo>();
        private readonly List<QueueConfig> _queues = new List<QueueConfig>();
        private readonly Dictionary<string, string> _replacementVariables = new Dictionary<string, string>();
        private readonly string _environmentPrefix = "";

        public MessageRegistry(IConfiguration configuration)
        {
            var environmentPrefix = configuration[$"Solace:EnvironmentPrefix"] ?? "";
            if (environmentPrefix != "")
            {
                if (!environmentPrefix.EndsWith("/"))
                    environmentPrefix += "/";
                _environmentPrefix = environmentPrefix;
            }
            LoadReplacementVariables(configuration);
            RegisterMessageTypes();
        }

        public IReadOnlyDictionary<Type, string> MessageTypes => _typeNameMap;
        public IReadOnlyDictionary<Type, PublishingInfo> PublishedMessages => _publishedMessages;
        public IReadOnlyList<QueueConfig> Queues => _queues;

        private void RegisterMessageTypes()
        {
            //Integration Events (Messages)
            RegisterQueue("General", queue =>
            {
                SubscribeViaTopic<AccountCreatedEvent>(queue, topicName: "General");
            }, maxFlows: 2);
            RegisterQueue("{Application}/CustomerCreatedEvent", queue =>
            {
                SubscribeViaTopic<CustomerCreatedEvent>(queue);
            });
            PublishToTopic<AccountCreatedEvent>(topicName: "General");
            PublishToTopic<CustomerCreatedEvent>();
            PublishToTopic<NotMappedEvent>();
            //Integration Commands
            RegisterQueue("PurchaseCreated", queue =>
            {
                SubscribeViaQueue<PurchaseCreated>(queue);
            });
            PublishToQueue<CreateLedger>(queueName: "Accounting/Ledgers");
            PublishToQueue<NotMappedIC>();
            PublishToQueue<PurchaseCreated>();
        }

        private void RegisterQueue(
            string logicalQueueName,
            Action<QueueConfig> registerSubscribers,
            int? maxFlows = null,
            string? selector = null)
        {
            if (selector != null)
            {
                selector = ResolveSelector(selector);
            }
            var queue = new QueueConfig(ResolveLocation(logicalQueueName), maxFlows, selector, new List<SubscribesToQueueInfo>());
            registerSubscribers(queue);
            _queues.Add(queue);
        }

        private void PublishToTopic<TMessage>(string? topicName = null, int? priority = null)
        {
            PublishInternal<TMessage>(topicName ?? GetDefaultMessageDestination<TMessage>(), priority);
        }

        private void PublishToQueue<TMessage>(string? queueName = null, int? priority = null)
        {
            PublishInternal<TMessage>(queueName ?? GetDefaultMessageDestination<TMessage>(), priority);
        }

        private void PublishInternal<TMessage>(string destination, int? priority = null)
        {
            AddMessageType<TMessage>();
            _publishedMessages.Add(typeof(TMessage), new PublishingInfo(typeof(TMessage), ResolveLocation(destination), priority));
        }

        private void SubscribeViaTopic<TMessage>(QueueConfig queue, string? topicName = null)
        {
            SubscribeInternal<TMessage>(queue, topicName ?? GetDefaultMessageDestination<TMessage>());
        }

        private void SubscribeViaQueue<TMessage>(QueueConfig queue)
        {
            SubscribeInternal<TMessage>(queue);
        }

        private void SubscribeInternal<TMessage>(QueueConfig queue, string? topicName = null)
        {
            AddMessageType<TMessage>();
            queue.SubscribedMessages.Add(new SubscribesToQueueInfo(
                    typeof(TMessage),
                    topicName != null ? ResolveLocation(topicName) : null
                    ));
        }

        private void AddMessageType<TMessage>()
        {
            if (!_typeNameMap.ContainsKey(typeof(TMessage)))
            {
                string messageTypeName = GetMessageTypeName<TMessage>();
                _typeNameMap.Add(typeof(TMessage), messageTypeName);
            }
        }

        private string GetDefaultMessageDestination<TMessage>()
        {
            var messageType = typeof(TMessage);
            string namepacePrefix = messageType.Namespace == null ? "" : $"{messageType.Namespace.Replace('.', '/')}/";
            return $"{namepacePrefix}{messageType.Name}";
        }

        private string GetMessageTypeName<TMessage>()
        {
            var type = typeof(TMessage);
            return $"{type.Namespace}.{type.Name}";
        }

        private void LoadReplacementVariables(IConfiguration configuration)
        {
            var config = configuration.GetSection("Solace").Get<SolaceConfiguration.SolaceConfig>();
            if (config == null) throw new Exception("No Solace configuration found in appsettings.json");

            var properties = config.GetType().GetProperties();
            foreach (var property in properties)
            {
                _replacementVariables.Add(property.Name, property.GetValue(config)?.ToString() ?? "");
            }
        }

        private string ReplaceVariables(string value)
        {
            if (value.Contains("{"))
            {
                foreach (var property in _replacementVariables)
                {
                    value = value.Replace($"{{{property.Key}}}", property.Value);
                }
            }
            return value;
        }

        private string ResolveSelector(string logicalSelector)
        {
            return ReplaceVariables(logicalSelector);
        }

        private string ResolveLocation(string logicalLocation)
        {
            return $"{_environmentPrefix}{ReplaceVariables(logicalLocation)}";
        }
    }

    public record PublishingInfo(Type PublishedMessage, string PublishTo, int? Priority);


    public record SubscribesToQueueInfo(Type MessageType, string? TopicName);


    public record QueueConfig(string QueueName, int? MaxFlows, string? Selector, List<SubscribesToQueueInfo> SubscribedMessages);
}