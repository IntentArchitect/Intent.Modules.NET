using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Entities.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Domain.Repositories.CollaborativeEditing
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDocumentRepository : IMongoRepository<Document, string>
    {
        [IntentManaged(Mode.Fully)]
        Task<Document?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Document>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}