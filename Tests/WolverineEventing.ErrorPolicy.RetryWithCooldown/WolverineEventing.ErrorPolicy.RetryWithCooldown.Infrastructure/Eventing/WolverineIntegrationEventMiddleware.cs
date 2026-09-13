using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using WolverineEventing.ErrorPolicy.RetryWithCooldown.Application.Common.Eventing;
using WolverineEventing.ErrorPolicy.RetryWithCooldown.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineIntegrationEventMiddleware", Version = "1.0")]

namespace WolverineEventing.ErrorPolicy.RetryWithCooldown.Infrastructure.Eventing
{
    public class WolverineIntegrationEventMiddleware
    {
        private readonly IMessageBus _messageBus;
        private readonly IUnitOfWork _unitOfWork;

        public WolverineIntegrationEventMiddleware(IMessageBus messageBus, IUnitOfWork unitOfWork)
        {
            _messageBus = messageBus;
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