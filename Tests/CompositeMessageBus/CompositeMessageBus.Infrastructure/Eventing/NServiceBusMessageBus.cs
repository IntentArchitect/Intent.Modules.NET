using CompositeMessageBus.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using NServiceBus;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageBus", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class NServiceBusMessageBus : IEventBus
    {
        public const string AddressKey = "address";
        private readonly List<object> _buffer = new();
        private readonly IMessageSession _messageSession;

        public NServiceBusMessageBus(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public object? ActiveContext { get; set; }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _buffer.Add(message);
        }

        public void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            throw new NotSupportedException("Publishing with additional data is not supported by this message bus provider.");
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _buffer.Add(message);
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
            if (_buffer.Count == 0)
            {
                return;
            }

            if (ActiveContext is IMessageHandlerContext handlerContext)
            {
                foreach (var message in _buffer)
                {
                    await handlerContext.Publish(message, cancellationToken);
                }
                _buffer.Clear();
                return;
            }

            foreach (var message in _buffer)
            {
                await _messageSession.Publish(message, cancellationToken);
            }

            _buffer.Clear();
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
    }
}