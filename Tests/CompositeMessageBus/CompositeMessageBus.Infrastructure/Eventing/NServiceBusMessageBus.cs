using System.Transactions;
using CompositeMessageBus.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageBus", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class NServiceBusMessageBus : IEventBus
    {
        public const string AddressKey = "address";
        private readonly List<object> _publishBuffer = new();
        private readonly List<object> _sendBuffer = new();
        private readonly IServiceProvider _serviceProvider;

        public NServiceBusMessageBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? ActiveContext { get; set; }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _publishBuffer.Add(message);
        }

        public void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            throw new NotSupportedException("Publishing with additional data is not supported by this message bus provider.");
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _sendBuffer.Add(message);
        }

        public void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            throw new NotSupportedException("Sending with additional data is not supported by this message bus provider.");
        }

        public void Send<TMessage>(TMessage message, Uri address)
            where TMessage : class
        {
            throw new NotSupportedException("Explicit address-based sending is not supported by this message bus provider.");
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_publishBuffer.Count == 0 && _sendBuffer.Count == 0)
            {
                return;
            }

            if (ActiveContext is IMessageHandlerContext handlerContext)
            {
                await DispatchAsync(m => handlerContext.Publish(m, new PublishOptions()), m => handlerContext.Send(m, new SendOptions()));
                return;
            }

            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {

                var messageSession = _serviceProvider.GetRequiredService<IMessageSession>();
                await DispatchAsync(m => messageSession.Publish(m, cancellationToken), m => messageSession.Send(m, cancellationToken));
            }
        }

        public void SchedulePublish<TMessage>(TMessage message, DateTime scheduled)
            where TMessage : class
        {
            throw new NotSupportedException("Scheduled publishing is not supported by this message bus provider.");
        }

        public void SchedulePublish<TMessage>(TMessage message, TimeSpan delay)
            where TMessage : class
        {
            throw new NotSupportedException("Scheduled publishing is not supported by this message bus provider.");
        }

        private async Task DispatchAsync(Func<object, Task> publishFn, Func<object, Task> sendFn)
        {
            foreach (var message in _publishBuffer)
            {
                await publishFn(message);
            }

            foreach (var message in _sendBuffer)
            {
                await sendFn(message);
            }

            _publishBuffer.Clear();
            _sendBuffer.Clear();
        }
    }
}