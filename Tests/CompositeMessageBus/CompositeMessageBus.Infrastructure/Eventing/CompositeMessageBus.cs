using CompositeMessageBus.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.CompositeMessageBus", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class CompositeMessageBus : IEventBus
    {
        private readonly MessageBrokerResolver _resolver;
        private readonly HashSet<IEventBus> _messageBusProviders = [];

        public CompositeMessageBus(MessageBrokerResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.Publish(message));
        }

        public void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.Publish(message, additionalData));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.Send(message));
        }

        public void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.Send(message, additionalData));
        }

        public void Send<TMessage>(TMessage message, Uri address)
            where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.Send(message, address));
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

        public void SchedulePublish<TMessage>(TMessage message, DateTime scheduled)
            where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.SchedulePublish(message, scheduled));
        }

        public void SchedulePublish<TMessage>(TMessage message, TimeSpan delay)
            where TMessage : class
        {
            ValidateMessageType<TMessage>();
            InnerDispatch<TMessage>(provider => provider.SchedulePublish(message, delay));
        }

        private void InnerDispatch<T>(Action<IEventBus> action)
        {
            var providers = _resolver.GetMessageBusProvidersForMessageType(typeof(T));

            foreach (var provider in providers)
            {
                _messageBusProviders.Add(provider);
                action(provider);
            }
        }

        private void ValidateMessageType<TMessage>()
        {
            var messageType = typeof(TMessage);

            if (!_resolver.IsMessageTypeRegistered(messageType))
            {
                throw new InvalidOperationException(
                    $"Message type '{messageType.FullName}' is not registered with any message broker provider. " +
                    $"Ensure the message is configured in the appropriate provider's configuration.");
            }
        }
    }
}