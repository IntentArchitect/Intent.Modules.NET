using System.Transactions;
using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineIntegrationEventMiddleware", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class WolverineIntegrationEventMiddleware
    {
        private readonly IEventBus _messageBus;
        private readonly IDaprStateStoreUnitOfWork _daprStateStoreUnitOfWork;
        private readonly IUnitOfWork _unitOfWork;

        public WolverineIntegrationEventMiddleware(IEventBus messageBus,
            IDaprStateStoreUnitOfWork daprStateStoreUnitOfWork,
            IUnitOfWork unitOfWork)
        {
            _messageBus = messageBus;
            _daprStateStoreUnitOfWork = daprStateStoreUnitOfWork ?? throw new ArgumentNullException(nameof(daprStateStoreUnitOfWork));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public Scope Before()
        {
            return new Scope
            {
                Transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled)
            };
        }

        public async Task AfterAsync(Scope scope, CancellationToken cancellationToken)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _daprStateStoreUnitOfWork.SaveChangesAsync(cancellationToken);

            scope.Transaction.Complete();
            scope.Committed = true;
        }

        public async Task FinallyAsync(Scope scope, CancellationToken cancellationToken)
        {
            scope.Transaction.Dispose();

            if (scope.Committed)
            {
                await _messageBus.FlushAllAsync(cancellationToken);
            }
        }

        public sealed class Scope
        {
            public TransactionScope Transaction { get; set; }
            public bool Committed { get; set; }
        }
    }
}