using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBUnitOfWorkInterface", Version = "1.0")]

namespace CosmosDB.Domain.Common.Interfaces
{
    public interface ICosmosDBUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}