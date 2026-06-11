using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.SQS.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageBus", Version = "1.0")]

namespace NServiceBus.SQS.Infrastructure.Eventing
{
    public class NServiceBusMessageBus : IMessageBus
    {
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
                await DispatchAsync(m => handlerContext.Publish(m, new PublishOptions()), m => handlerContext.Send(m, new SendOptions()));
                return;
            }

            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {

                var messageSession = _serviceProvider.GetRequiredService<IMessageSession>();
                await DispatchAsync(m => messageSession.Publish(m, cancellationToken), m => messageSession.Send(m, cancellationToken));
            }
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