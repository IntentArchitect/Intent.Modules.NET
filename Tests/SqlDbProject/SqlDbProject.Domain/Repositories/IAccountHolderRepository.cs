using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccountHolderRepository : IEFRepository<AccountHolder, AccountHolder>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(long accountHolderId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccountHolder?> FindByIdAsync(long accountHolderId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccountHolder?> FindByIdAsync(long accountHolderId, Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccountHolder>> FindByIdsAsync(long[] accountHolderIds, CancellationToken cancellationToken = default);
    }
}