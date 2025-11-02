using System.Text.Json;
using CleanArchitecture.QueueStorage.Application.Common.Eventing;
using CleanArchitecture.QueueStorage.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageEventDispatcher", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure.Eventing
{
    public class AzureQueueStorageEventDispatcher : IAzureQueueStorageEventDispatcher
    {
        private readonly Dictionary<string, DispatchHandler> _handlers;

        public AzureQueueStorageEventDispatcher(IOptions<AzureQueueStorageSubscriptionOptions> options)
        {
            _handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);
        }

        public async Task DispatchAsync(
            IServiceProvider serviceProvider,
            AzureQueueStorageEnvelope message,
            JsonSerializerOptions serializerOptions,
            CancellationToken cancellationToken)
        {
            var messageTypeName = message.MessageType;

            if (_handlers.TryGetValue(messageTypeName, out var handlerAsync))
            {
                await handlerAsync(serviceProvider, message, serializerOptions, cancellationToken);
            }
        }

        public static async Task InvokeDispatchHandler<TMessage, THandler>(
            IServiceProvider serviceProvider,
            AzureQueueStorageEnvelope message,
            JsonSerializerOptions serializerOptions,
            CancellationToken cancellationToken)
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            var messageObj = ((JsonElement)message.Payload).Deserialize<TMessage>(serializerOptions);
            var handler = serviceProvider.GetRequiredService<THandler>();
            await handler.HandleAsync(messageObj, cancellationToken);
        }
    }
}