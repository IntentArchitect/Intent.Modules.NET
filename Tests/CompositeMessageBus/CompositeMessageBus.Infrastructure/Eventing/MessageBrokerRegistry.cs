using CompositeMessageBus.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.MessageBrokerRegistry", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class MessageBrokerRegistry
    {
        private readonly Dictionary<Type, List<Type>> _messageTypeToProviderTypes = new();
        private IReadOnlyDictionary<Type, IReadOnlyList<Type>>? _readOnlyCache;

        public void Register<TMessage, TMessageBus>()
            where TMessage : class
            where TMessageBus : IEventBus
        {
            if (typeof(TMessageBus) == typeof(CompositeMessageBus))
            {
                throw new InvalidOperationException("Cannot register CompositeMessageBus as a message bus provider.");
            }
            Register(typeof(TMessage), typeof(TMessageBus));
        }

        private void Register(Type messageType, Type providerType)
        {
            if (!_messageTypeToProviderTypes.TryGetValue(messageType, out var providerTypes))
            {
                providerTypes = new List<Type>();
                _messageTypeToProviderTypes[messageType] = providerTypes;
            }



            if (!providerTypes.Contains(providerType))
            {
                providerTypes.Add(providerType);
            }

            // Invalidate cache when new registrations are made
            _readOnlyCache = null;
        }

        public IReadOnlyDictionary<Type, IReadOnlyList<Type>> GetAllRegistrations()
        {
            EnsureReadOnlyCache();
            return _readOnlyCache!;
        }

        private void EnsureReadOnlyCache()
        {
            _readOnlyCache ??= _messageTypeToProviderTypes.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<Type>)kvp.Value.AsReadOnly());
        }
    }
}