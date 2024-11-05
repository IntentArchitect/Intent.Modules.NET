using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Application.Common.Eventing;
using Kafka.Consumer.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Kafka.KafkaEventDispatcher", Version = "1.0")]

namespace Kafka.Consumer.Infrastructure.Eventing
{
    public class KafkaEventDispatcher<T> : IKafkaEventDispatcher<T>
        where T : class
    {
        private readonly IEventBus _eventBus;
        private readonly IIntegrationEventHandler<T> _handler;
        private readonly IUnitOfWork _unitOfWork;

        public KafkaEventDispatcher(IEventBus eventBus, IIntegrationEventHandler<T> handler, IUnitOfWork unitOfWork)
        {
            _eventBus = eventBus;
            _handler = handler;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Dispatch(T message, CancellationToken cancellationToken = default)
        {
            // The execution is wrapped in a transaction scope to ensure that if any other
            // SaveChanges calls to the data source (e.g. EF Core) are called, that they are
            // transacted atomically. The isolation is set to ReadCommitted by default (i.e. read-
            // locks are released, while write-locks are maintained for the duration of the
            // transaction). Learn more on this approach for EF Core:
            // https://docs.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _handler.HandleAsync(message, cancellationToken);

                // By calling SaveChanges at the last point in the transaction ensures that write-
                // locks in the database are created and then released as quickly as possible. This
                // helps optimize the application to handle a higher degree of concurrency.
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Commit transaction if everything succeeds, transaction will auto-rollback when
                // disposed if anything failed.
                transaction.Complete();
            }

            await _eventBus.FlushAllAsync(cancellationToken);
        }
    }
}