using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AzureFunction.QueueStorage.Application.Common.Eventing;
using AzureFunction.QueueStorage.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageSubscriptionOptions", Version = "1.0")]

namespace AzureFunction.QueueStorage.Infrastructure.Configuration
{

    public class AzureQueueStorageSubscriptionOptions
    {
        private readonly List<SubscriptionEntry> _entries = new List<SubscriptionEntry>();

        public IReadOnlyList<SubscriptionEntry> Entries => _entries;

        public void Add<TMessage, THandler>(string queueName)
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            ArgumentNullException.ThrowIfNull(queueName);
            _entries.Add(new SubscriptionEntry(typeof(TMessage), AzureQueueStorageEventDispatcher.InvokeDispatchHandler<TMessage, THandler>, queueName));
        }
    }

    public delegate Task DispatchHandler(IServiceProvider serviceProvider, AzureQueueStorageEnvelope message, JsonSerializerOptions serializationOptions, CancellationToken cancellationToken);

    public record SubscriptionEntry(Type MessageType, DispatchHandler HandlerAsync, string QueueName);
}