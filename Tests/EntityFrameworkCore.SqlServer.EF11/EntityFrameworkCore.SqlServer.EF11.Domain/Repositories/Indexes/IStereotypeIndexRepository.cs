using EntityFrameworkCore.SqlServer.EF11.Domain.Indexes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IStereotypeIndexRepository : IEFRepository<StereotypeIndex, StereotypeIndex>
    {
        [IntentManaged(Mode.Fully)]
        Task<StereotypeIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<StereotypeIndex?> FindByIdAsync(Guid id, Func<IQueryable<StereotypeIndex>, IQueryable<StereotypeIndex>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<StereotypeIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}