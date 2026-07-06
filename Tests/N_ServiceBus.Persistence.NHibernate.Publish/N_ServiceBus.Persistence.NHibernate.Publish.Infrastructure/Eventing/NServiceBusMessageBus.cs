using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using N_ServiceBus.Persistence.NHibernate.Publish.Application.Common.Eventing;
using NServiceBus.TransactionalSession;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageBus", Version = "1.0")]

namespace N_ServiceBus.Persistence.NHibernate.Publish.Infrastructure.Eventing
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
                var transactionalSession = _serviceProvider.GetRequiredService<ITransactionalSession>();

                await transactionalSession.Open(new NHibernateOpenSessionOptions(), cancellationToken);
                await DispatchAsync(m => transactionalSession.Publish(m, cancellationToken), m => transactionalSession.Send(m, cancellationToken));

                await transactionalSession.Commit(cancellationToken);
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