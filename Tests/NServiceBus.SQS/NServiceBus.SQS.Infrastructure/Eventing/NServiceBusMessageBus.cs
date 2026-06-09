using Intent.RoslynWeaver.Attributes;
using NServiceBus.SQS.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageBus", Version = "1.0")]

namespace NServiceBus.SQS.Infrastructure.Eventing
{
    public class NServiceBusMessageBus : IMessageBus
    {
        private readonly List<object> _publishBuffer = new();
        private readonly List<object> _sendBuffer = new();
        private readonly IMessageSession _messageSession;

        public NServiceBusMessageBus(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public object? ActiveContext { get; set; }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _publishBuffer.Add(message);
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _sendBuffer.Add(message);
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_publishBuffer.Count == 0 && _sendBuffer.Count == 0)
            {
                return;
            }

            if (ActiveContext is IMessageHandlerContext handlerContext)
            {
                foreach (var message in _publishBuffer)
                {
                    await handlerContext.Publish(message, new PublishOptions());
                }

                foreach (var message in _sendBuffer)
                {
                    await handlerContext.Send(message, new SendOptions());
                }
                _publishBuffer.Clear();
                _sendBuffer.Clear();
                return;
            }

            foreach (var message in _publishBuffer)
            {
                await _messageSession.Publish(message, cancellationToken);
            }

            foreach (var message in _sendBuffer)
            {
                await _messageSession.Send(message, cancellationToken);
            }

            _publishBuffer.Clear();
            _sendBuffer.Clear();
        }
    }
}