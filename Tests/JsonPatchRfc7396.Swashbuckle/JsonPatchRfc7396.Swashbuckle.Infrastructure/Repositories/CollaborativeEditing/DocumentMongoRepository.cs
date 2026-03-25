using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing;
using JsonPatchRfc7396.Swashbuckle.Domain.Repositories.CollaborativeEditing;
using JsonPatchRfc7396.Swashbuckle.Infrastructure.Persistence;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Infrastructure.Repositories.CollaborativeEditing
{
    internal class DocumentMongoRepository : MongoRepositoryBase<Document, string>, IDocumentRepository
    {
        public DocumentMongoRepository(IMongoCollection<Document> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork, x => x.Id)
        {
        }

        public Task<Document?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => FindAsync(x => x.Id == id, cancellationToken);

        public Task<List<Document>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default) => FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
    }
}