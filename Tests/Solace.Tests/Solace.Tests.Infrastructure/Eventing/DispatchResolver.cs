using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.DispatchResolver", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Eventing
{
    public class DispatchResolver
    {
        private readonly Dictionary<Type, Type> _dispatcherMappings;

        public DispatchResolver(MessageRegistry messageRegistry)
        {
            _dispatcherMappings = new Dictionary<Type, Type>();
            foreach (var queue in messageRegistry.Queues)
            {
                foreach (var message in queue.SubscribedMessages)
                {
                    var genericTypeDefinition = typeof(ISolaceEventDispatcher<>);
                    var closedConstructedType = genericTypeDefinition.MakeGenericType(message.MessageType);
                    _dispatcherMappings.Add(message.MessageType, closedConstructedType);
                }
            }
        }

        public object ResolveDispatcher(Type messageType, IServiceProvider scopedProvider)
        {
            if (_dispatcherMappings.TryGetValue(messageType, out var dispatcherType))
            {
                return scopedProvider.GetRequiredService(dispatcherType);
            }

            throw new InvalidOperationException($"Dispatcher for type {messageType.Name} not found");
        }
    }
}