using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageUnitOfWorkInterface", Version = "1.0")]

namespace TableStorage.Tests.Domain.Common.Interfaces
{
    public interface ITableStorageUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}