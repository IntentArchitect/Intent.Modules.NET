using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using N_ServiceBus.Persistence.Sql.Subscribe.Application.Common.Eventing;
using N_ServiceBus.Persistence.Sql.Subscribe.Infrastructure.Persistence;
using NServiceBus.Persistence.Sql;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusMessageHandler", Version = "1.0")]

namespace N_ServiceBus.Persistence.Sql.Subscribe.Infrastructure.Eventing
{
    internal class NServiceBusMessageHandler<TMessage> : IHandleMessages<TMessage>
        where TMessage : class
    {
        private readonly IIntegrationEventHandler<TMessage> _handler;
        private readonly ApplicationDbContext _dbContext;
        private readonly NServiceBusMessageBus _messageBus;

        public NServiceBusMessageHandler(IIntegrationEventHandler<TMessage> handler,
            ApplicationDbContext dbContext,
            NServiceBusMessageBus messageBus)
        {
            _handler = handler;
            _dbContext = dbContext;
            _messageBus = messageBus;
        }

        public async Task Handle(TMessage message, IMessageHandlerContext context)
        {
            _messageBus.ActiveContext = context;

            var sqlSession = context.SynchronizedStorageSession.SqlPersistenceSession();
            _dbContext.Database.SetDbConnection(sqlSession.Connection);
            await _dbContext.Database.UseTransactionAsync((System.Data.Common.DbTransaction)sqlSession.Transaction, context.CancellationToken);

            await _handler.HandleAsync(message, context.CancellationToken);
            await _dbContext.SaveChangesAsync(context.CancellationToken);
            await _messageBus.FlushAllAsync(context.CancellationToken);
        }
    }
}