using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Transport.AmazonSqs.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.UnitOfWorkMiddleware", Version = "1.0")]

namespace WolverineEventing.Transport.AmazonSqs.Infrastructure.Dispatch.Middleware
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