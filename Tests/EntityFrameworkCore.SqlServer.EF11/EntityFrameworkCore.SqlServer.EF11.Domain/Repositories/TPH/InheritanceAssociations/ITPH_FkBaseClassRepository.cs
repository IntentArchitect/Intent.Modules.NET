using EntityFrameworkCore.SqlServer.EF11.Domain.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_FkBaseClassRepository : IEFRepository<TPH_FkBaseClass, TPH_FkBaseClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_FkBaseClass?> FindByIdAsync((Guid CompositeKeyA, Guid CompositeKeyB) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_FkBaseClass?> FindByIdAsync((Guid CompositeKeyA, Guid CompositeKeyB) id, Func<IQueryable<TPH_FkBaseClass>, IQueryable<TPH_FkBaseClass>> queryOptions, CancellationToken cancellationToken = default);
    }
}