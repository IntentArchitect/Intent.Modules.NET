using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.SubscriptionOptions", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Infrastructure.Configuration
{
    public class SubscriptionOptions
    {
        private readonly List<SubscriptionEntry> _entries = [];

        public IReadOnlyList<SubscriptionEntry> Entries => _entries;

        public void Add<TMessage, THandler>()
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            _entries.Add(new SubscriptionEntry(typeof(TMessage), AzureServiceBusMessageDispatcher.InvokeDispatchHandler<TMessage, THandler>));
        }
    }

    public delegate Task DispatchHandler(IServiceProvider serviceProvider, ServiceBusReceivedMessage message, CancellationToken cancellationToken);

    public record SubscriptionEntry(Type MessageType, DispatchHandler HandlerAsync);
}