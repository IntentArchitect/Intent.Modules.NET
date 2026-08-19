using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageUnitOfWorkInterface", Version = "1.0")]

namespace AzureIdentityManagement.Domain.Common.Interfaces
{
    public interface ITableStorageUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}