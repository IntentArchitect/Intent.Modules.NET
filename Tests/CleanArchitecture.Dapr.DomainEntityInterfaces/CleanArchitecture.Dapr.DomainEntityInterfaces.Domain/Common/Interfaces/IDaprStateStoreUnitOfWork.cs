using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreUnitOfWorkInterface", Version = "1.0")]

namespace CleanArchitecture.Dapr.DomainEntityInterfaces.Domain.Common.Interfaces
{
    public interface IDaprStateStoreUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}