using EntityFrameworkCore.SqlServer.EF11.Domain.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_FkBaseClassAssociatedRepository : IEFRepository<TPH_FkBaseClassAssociated, TPH_FkBaseClassAssociated>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_FkBaseClassAssociated?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_FkBaseClassAssociated?> FindByIdAsync(Guid id, Func<IQueryable<TPH_FkBaseClassAssociated>, IQueryable<TPH_FkBaseClassAssociated>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_FkBaseClassAssociated>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}