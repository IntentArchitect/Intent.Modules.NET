using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using NServiceBus.OutboxPattern.Subscribe.Application.Common.Eventing;
using NServiceBus.OutboxPattern.Subscribe.Infrastructure.Persistence;
using NServiceBus.Persistence.Sql;
using NServiceBus.TransactionalSession;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageBus", Version = "1.0")]

namespace NServiceBus.OutboxPattern.Subscribe.Infrastructure.Eventing
{
    public class NServiceBusMessageBus : IMessageBus
    {
        private readonly List<object> _publishBuffer = new();
        private readonly List<object> _sendBuffer = new();
        private readonly ITransactionalSession _transactionalSession;
        private readonly ApplicationDbContext _dbContext;

        public NServiceBusMessageBus(ITransactionalSession transactionalSession, ApplicationDbContext dbContext)
        {
            _transactionalSession = transactionalSession;
            _dbContext = dbContext;
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

            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _transactionalSession.Open(new SqlPersistenceOpenSessionOptions(), cancellationToken);
                var sqlSession = _transactionalSession.SynchronizedStorageSession.SqlPersistenceSession();
                _dbContext.Database.SetDbConnection(sqlSession.Connection);
                await _dbContext.Database.UseTransactionAsync((System.Data.Common.DbTransaction)sqlSession.Transaction, cancellationToken);

                try
                {
                    foreach (var message in _publishBuffer)
                    {
                        await _transactionalSession.Publish(message, cancellationToken);
                    }

                    foreach (var message in _sendBuffer)
                    {
                        await _transactionalSession.Send(message, cancellationToken);
                    }

                    await _dbContext.SaveChangesAsync(cancellationToken);
                    await _transactionalSession.Commit(cancellationToken);

                    _publishBuffer.Clear();
                    _sendBuffer.Clear();
                }
                finally
                {
                    _dbContext.Database.SetDbConnection(null);
                }
            }
        }
    }
}