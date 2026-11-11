using EntityFrameworkCore.SqlServer.EF11.Domain.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_MiddleAbstract_LeafRepository : IEFRepository<TPH_MiddleAbstract_Leaf, TPH_MiddleAbstract_Leaf>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_MiddleAbstract_Leaf?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_MiddleAbstract_Leaf?> FindByIdAsync(Guid id, Func<IQueryable<TPH_MiddleAbstract_Leaf>, IQueryable<TPH_MiddleAbstract_Leaf>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_MiddleAbstract_Leaf>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}