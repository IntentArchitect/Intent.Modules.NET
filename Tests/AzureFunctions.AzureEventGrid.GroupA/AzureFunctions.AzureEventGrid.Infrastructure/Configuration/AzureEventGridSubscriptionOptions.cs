using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridSubscriptionOptions", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Configuration
{
    public class AzureEventGridSubscriptionOptions
    {
        private readonly List<SubscriptionEntry> _entries = [];

        public IReadOnlyList<SubscriptionEntry> Entries => _entries;

        public void Add<TMessage, THandler>()
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            _entries.Add(new SubscriptionEntry(typeof(TMessage), AzureEventGridMessageDispatcher.InvokeDispatchHandler<TMessage, THandler>));
        }
    }

    public delegate Task DispatchHandler(IServiceProvider serviceProvider, CloudEvent cloudEvent, CancellationToken cancellationToken);

    public record SubscriptionEntry(Type MessageType, DispatchHandler HandlerAsync);
}