using Intent.RoslynWeaver.Attributes;
using NServiceBus;
using NServiceBus.LearnerTransport.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageBus", Version = "1.0")]

namespace NServiceBus.LearnerTransport.Infrastructure.Eventing
{
    public class NServiceBusMessageBus : IMessageBus
    {
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

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _buffer.Add(message);
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
    }
}