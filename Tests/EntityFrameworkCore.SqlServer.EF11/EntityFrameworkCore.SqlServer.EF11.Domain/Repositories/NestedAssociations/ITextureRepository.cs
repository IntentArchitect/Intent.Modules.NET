using EntityFrameworkCore.SqlServer.EF11.Domain.NestedAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.NestedAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITextureRepository : IEFRepository<Texture, Texture>
    {
        [IntentManaged(Mode.Fully)]
        Task<Texture?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Texture?> FindByIdAsync(Guid id, Func<IQueryable<Texture>, IQueryable<Texture>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Texture>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}