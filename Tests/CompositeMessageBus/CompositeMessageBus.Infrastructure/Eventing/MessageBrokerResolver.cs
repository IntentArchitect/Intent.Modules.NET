using CompositeMessageBus.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.MessageBrokerResolver", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
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

        public IReadOnlyList<IEventBus> GetMessageBusProvidersForMessageType(Type messageType)
        {
            if (!_registry.GetAllRegistrations().TryGetValue(messageType, out var providerTypes))
            {
                return [];
            }

            return providerTypes
                .Select(pt => (IEventBus)_serviceProvider.GetRequiredService(pt))
                .ToList();
        }
    }
}