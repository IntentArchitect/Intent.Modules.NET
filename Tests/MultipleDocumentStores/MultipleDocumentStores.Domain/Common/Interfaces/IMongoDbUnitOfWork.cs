using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbUnitOfWorkInterface", Version = "1.0")]

namespace MultipleDocumentStores.Domain.Common.Interfaces
{
    public interface IMongoDbUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}