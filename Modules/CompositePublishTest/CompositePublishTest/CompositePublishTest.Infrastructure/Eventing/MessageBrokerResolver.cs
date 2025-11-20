using System;
using System.Collections.Generic;
using System.Linq;
using CompositePublishTest.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CompositePublishTest.Infrastructure.Eventing
{
    /// <summary>
    /// Scoped resolver that efficiently resolves IMessageBrokerProvider instances for message types.
    /// Uses IServiceProvider to resolve providers and caches them per scope.
    /// </summary>
    public class MessageBrokerResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageBrokerRegistry _registry;

        public MessageBrokerResolver(IServiceProvider serviceProvider, MessageBrokerRegistry registry)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }
        
        public bool IsMessageTypeRegistered(Type messageType)
        {
            return _registry.GetAllRegistrations().ContainsKey(messageType);
        }

        public IReadOnlyList<IMessageBus> GetMessageBusProvidersForMessageType(Type messageType)
        {
            if (!_registry.GetAllRegistrations().TryGetValue(messageType, out var providerTypes))
            {
                return [];
            }
            
            return providerTypes
                .Select(pt => (IMessageBus)_serviceProvider.GetRequiredService(pt))
                .ToList();
        }
    }
}
