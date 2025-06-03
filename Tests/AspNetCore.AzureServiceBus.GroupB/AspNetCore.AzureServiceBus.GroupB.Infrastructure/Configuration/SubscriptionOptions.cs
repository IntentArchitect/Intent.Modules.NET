using AspNetCore.AzureServiceBus.GroupB.Application.Common.Eventing;
using AspNetCore.AzureServiceBus.GroupB.Infrastructure.Eventing;
using Azure.Messaging.ServiceBus;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.SubscriptionOptions", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupB.Infrastructure.Configuration
{
    public class SubscriptionOptions
    {
        private readonly List<SubscriptionEntry> _entries = [];

        public IReadOnlyList<SubscriptionEntry> Entries => _entries;

        public void Add<TMessage, THandler>(string queueOrTopicName, string? subscriptionName = null)
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            ArgumentNullException.ThrowIfNull(queueOrTopicName);
            _entries.Add(new SubscriptionEntry(typeof(TMessage), AzureServiceBusMessageDispatcher.InvokeDispatchHandler<TMessage, THandler>, queueOrTopicName, subscriptionName));
        }
    }

    public delegate Task DispatchHandler(IServiceProvider serviceProvider, ServiceBusReceivedMessage message, CancellationToken cancellationToken);

    public record SubscriptionEntry(Type MessageType, DispatchHandler HandlerAsync, string QueueOrTopicName, string? SubscriptionName);
}