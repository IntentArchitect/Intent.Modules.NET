using EntityFrameworkCore.SqlServer.EF11.Domain.Accounts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccViewOverrideRepository : IEFRepository<AccViewOverride, AccViewOverride>
    {
        [IntentManaged(Mode.Fully)]
        Task<AccViewOverride?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccViewOverride?> FindByIdAsync(Guid id, Func<IQueryable<AccViewOverride>, IQueryable<AccViewOverride>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccViewOverride>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}