using Intent.RoslynWeaver.Attributes;
using System.Transactions;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Wolverine.CQRS.TestApplication.Application.Common.Behaviours
{
    public class UnitOfWorkMiddleware
    {
        // 1. BEFORE THE HANDLER RUNS
        // If an external transaction is active, we return null to tell the 'After' step to skip TransactionScope.
        public static TransactionScope? Before(IUnitOfWork dataSource)
        {
            if (dataSource.HasDbTransaction())
            {
                return null;
            }

            return new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled);
        }

        // 2. AFTER THE HANDLER COMPLETES
        // Wolverine automatically matches 'TransactionScope? tx' from what was returned by Before()
        public static async Task AfterAsync(
            TransactionScope? tx,
            IUnitOfWork dataSource,
            CancellationToken cancellationToken)
        {
            try
            {
                // Save changes to primary data source
                await dataSource.SaveChangesAsync(cancellationToken);

                // Commit the transaction scope if it was initialized in Before()
                tx?.Complete();
            }
            finally
            {
                // Ensure disposal occurs cleanly
                tx?.Dispose();
            }
        }
    }
}

