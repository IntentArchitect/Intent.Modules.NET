using System;
using System.Collections.Generic;
using System.Linq;
using CompositePublishTest.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CompositePublishTest.Infrastructure.Eventing
{
    /// <summary>
    /// Central registry that maps message types to their provider types.
    /// Mutable during startup configuration, immutable at runtime.
    /// Registered as a singleton.
    /// </summary>
    public class MessageBrokerRegistry
    {
        private readonly Dictionary<Type, List<Type>> _messageTypeToProviderTypes = new();
        private IReadOnlyDictionary<Type, IReadOnlyList<Type>>? _readOnlyCache;

        /// <summary>
        /// Registers a message type to be handled by a specific provider type using generics.
        /// Convenience overload that enforces the provider implements <see cref="IMessageBus"/>.
        /// </summary>
        /// <typeparam name="TMessage">The message type to register.</typeparam>
        /// <typeparam name="TMessageBus">The provider type that will handle the message.</typeparam>
        public void Register<TMessage, TMessageBus>()
            where TMessage : class
            where TMessageBus : IMessageBus
        {
            if (typeof(TMessageBus) == typeof(CompositeMessageBus))
            {
                throw new InvalidOperationException("Cannot register CompositeMessageBus as a message bus provider.");
            }
            Register(typeof(TMessage), typeof(TMessageBus));
        }

        /// <summary>
        /// Registers a message type to be handled by a specific provider type.
        /// Supports fan-out: multiple providers can handle the same message type.
        /// </summary>
        /// <param name="messageType">The message type to register.</param>
        /// <param name="providerType">The type of the IMessageBrokerProvider that will handle this message.</param>
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
        
        /// <summary>
        /// Gets the provider types registered for the given message type.
        /// This is cached for efficiency.
        /// </summary>
        /// <returns>
        /// A read-only list of provider types that handle the specified message type.
        /// </returns>
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
