using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbUnitOfWorkInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Common.Interfaces
{
    public interface IMongoDbUnitOfWork : IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}