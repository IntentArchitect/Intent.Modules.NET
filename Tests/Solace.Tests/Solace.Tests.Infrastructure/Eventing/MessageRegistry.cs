using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Solace.Tests.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.MessageRegistry", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Eventing
{
    public class MessageRegistry
    {
        private readonly List<MessageConfiguration> _messageTypes;
        private readonly Dictionary<Type, MessageConfiguration> _typeLookup;
        private readonly string _environmentPrefix = "";
        private readonly string _applicationPrefix = "";

        public MessageRegistry(IConfiguration configuration)
        {
            var environmentPrefix = configuration[$"Solace:EnvironmentPrefix"] ?? "";
            if (environmentPrefix != "")
            {
                if (!environmentPrefix.EndsWith("/"))
                    environmentPrefix += "/";
                _environmentPrefix = environmentPrefix;
            }
            var applicationPrefix = configuration[$"Solace:Application"] ?? "";
            if (applicationPrefix != "")
            {
                if (!applicationPrefix.EndsWith("/"))
                    applicationPrefix += "/";
                _applicationPrefix = applicationPrefix;
            }

            _messageTypes = new List<MessageConfiguration>();
            LoadMessageTypes();
            _typeLookup = new Dictionary<Type, MessageConfiguration>();
            foreach (var message in _messageTypes)
            {
                _typeLookup.Add(message.MessageType, message);
            }
        }

        public IReadOnlyList<MessageConfiguration> MessageTypes => _messageTypes;

        private void LoadMessageTypes()
        {
            //Integration Events (Messages)
            _messageTypes.Add(MessageConfiguration.Create<AccountCreatedEvent>(
                GetDestination<AccountCreatedEvent>("General"),
                GetDestination<AccountCreatedEvent>("General", SubscriptionType.ViaTopic)));
            _messageTypes.Add(GetDefaultMessageConfig<CustomerCreatedEvent>(SubscriptionType.ViaTopic));
            //Integration Commands
            _messageTypes.Add(GetDefaultMessageConfig<PurchaseCreated>(SubscriptionType.ViaQueue));
        }

        public MessageConfiguration GetConfig(Type messageType)
        {
            return _typeLookup[messageType];
        }

        public MessageConfiguration GetConfig<TMessage>()
        {
            return GetConfig(typeof(TMessage));
        }

        private MessageConfiguration GetDefaultMessageConfig<TMessage>(SubscriptionType subscriptionType = SubscriptionType.None)
        {
            switch (subscriptionType)
            {
                case SubscriptionType.ViaTopic:
                    return MessageConfiguration.Create<TMessage>(GetDefaultDestination<TMessage>(),
                            GetDefaultDestination<TMessage>(SubscriptionType.ViaTopic));
                case SubscriptionType.ViaQueue:
                    return MessageConfiguration.Create<TMessage>(GetDefaultDestination<TMessage>(),
                            GetDefaultDestination<TMessage>(SubscriptionType.ViaQueue));
                case SubscriptionType.None:
                default:
                    return MessageConfiguration.Create<TMessage>(GetDefaultDestination<TMessage>());
            }
        }

        private string GetDestination<TMessage>(
            string logicalPath,
            SubscriptionType subscriptionType = SubscriptionType.None)
        {
            var messageType = typeof(TMessage);
            switch (subscriptionType)
            {
                case SubscriptionType.ViaTopic:
                    return $"{_environmentPrefix}{_applicationPrefix}{logicalPath}";
                case SubscriptionType.ViaQueue:
                case SubscriptionType.None:
                default:
                    return $"{_environmentPrefix}{logicalPath}";
            }
        }

        private string GetDefaultDestination<TMessage>(SubscriptionType subscriptionType = SubscriptionType.None)
        {
            var messageType = typeof(TMessage);
            string namepacePrefix = messageType.Namespace == null ? "" : $"{messageType.Namespace.Replace('.', '/')}/";
            return GetDestination<TMessage>($"{namepacePrefix}{messageType.Name}", subscriptionType);
        }
    }

    public enum SubscriptionType
    {
        None,

        ViaTopic,

        ViaQueue
    }
}