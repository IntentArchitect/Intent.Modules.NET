using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CompositePublishTest.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CompositePublishTest.Infrastructure.Eventing
{
    public class CompositeMessageBus : IMessageBus
    {
        private readonly MessageBrokerResolver _resolver;
        private readonly HashSet<IMessageBus> _messageBusProviders = [];

        public CompositeMessageBus(MessageBrokerResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }
        
        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.Publish(message));
        }
        
        public void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData) where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.Publish(message, additionalData));
        }
        
        public void Send<TMessage>(TMessage message) where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.Send(message));
        }
        
        public void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData) where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.Send(message, additionalData));
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_messageBusProviders.Count == 0)
            {
                return;
            }

            await Task.WhenAll(_messageBusProviders.Select(provider => provider.FlushAllAsync(cancellationToken)));
            _messageBusProviders.Clear();
        }
        
        private void InnerDispatch<T>(Action<IMessageBus> action)
        {
            var providers = _resolver.GetMessageBusProvidersForMessageType(typeof(T));
            foreach (var provider in providers)
            {
                _messageBusProviders.Add(provider);
                action(provider);
            }
        }

        /// <summary>
        /// Validates that a message type is registered with at least one provider.
        /// Throws InvalidOperationException if not registered (fail-fast).
        /// </summary>
        private void ValidateMessageType<TMessage>()
        {
            var messageType = typeof(TMessage);
            if (!_resolver.IsMessageTypeRegistered(messageType))
            {
                throw new InvalidOperationException(
                    $"Message type '{messageType.FullName}' is not registered with any message broker provider. " +
                    $"Ensure the message is configured in the appropriate provider's configuration (e.g., AzureEventGridPublisherOptions).");
            }
        }
    }
}
