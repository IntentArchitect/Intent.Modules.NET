using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbUnitOfWorkInterface", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.Common.Interfaces
{
    public interface IMongoDbUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}