using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.UnitOfWorkMiddleware", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Common.Behaviours
{
    public class UnitOfWorkMiddleware
    {
        public static TransactionScope? Before(IUnitOfWork dataSource)
        {
            return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        }

        public static async Task AfterAsync(
            TransactionScope? tx,
            IUnitOfWork dataSource,
            CancellationToken cancellationToken)
        {
            try
            {
                await dataSource.SaveChangesAsync(cancellationToken);
                tx?.Complete();
            }
            finally
            {
                tx?.Dispose();
            }
        }
    }
}

