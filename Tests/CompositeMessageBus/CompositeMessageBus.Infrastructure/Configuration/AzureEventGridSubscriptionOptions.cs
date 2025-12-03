using Azure.Messaging;
using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridSubscriptionOptions", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public class AzureEventGridSubscriptionOptions
    {
        private readonly List<AzureEventGridSubscriptionEntry> _entries = [];

        public IReadOnlyList<AzureEventGridSubscriptionEntry> Entries => _entries;

        public void Add<TMessage, THandler>()
            where TMessage : class
            where THandler : IIntegrationEventHandler<TMessage>
        {
            _entries.Add(new AzureEventGridSubscriptionEntry(typeof(TMessage), AzureEventGridMessageDispatcher.InvokeDispatchHandler<TMessage, THandler>));
        }
    }

    public delegate Task AzureEventGridDispatchHandler(IServiceProvider serviceProvider, CloudEvent cloudEvent, CancellationToken cancellationToken);

    public record AzureEventGridSubscriptionEntry(Type MessageType, AzureEventGridDispatchHandler HandlerAsync);
}