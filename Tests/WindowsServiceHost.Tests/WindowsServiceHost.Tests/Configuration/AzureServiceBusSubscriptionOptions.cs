using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Intent.RoslynWeaver.Attributes;
using WindowsServiceHost.Tests.Common.Eventing;
using WindowsServiceHost.Tests.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusSubscriptionOptions", Version = "1.0")]

namespace WindowsServiceHost.Tests.Configuration
{
    public class AzureServiceBusSubscriptionOptions
    {
        private readonly List<AzureServiceBusSubscriptionEntry> _entries = [];

        public IReadOnlyList<AzureServiceBusSubscriptionEntry> Entries => _entries;

        public void Add<TMessage, THandler>(string queueOrTopicName, string? subscriptionName = null)
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            ArgumentNullException.ThrowIfNull(queueOrTopicName);
            _entries.Add(new AzureServiceBusSubscriptionEntry(typeof(TMessage), AzureServiceBusMessageDispatcher.InvokeDispatchHandler<TMessage, THandler>, queueOrTopicName, subscriptionName));
        }
    }

    public delegate Task AzureServiceBusDispatchHandler(IServiceProvider serviceProvider, ServiceBusReceivedMessage message, CancellationToken cancellationToken);

    public record AzureServiceBusSubscriptionEntry(Type MessageType, AzureServiceBusDispatchHandler HandlerAsync, string QueueOrTopicName, string? SubscriptionName);
}