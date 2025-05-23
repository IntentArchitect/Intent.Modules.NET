using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccountRepository : IEFRepository<Account, Account>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(long accountId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Account?> FindByIdAsync(long accountId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Account?> FindByIdAsync(long accountId, Func<IQueryable<Account>, IQueryable<Account>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Account>> FindByIdsAsync(long[] accountIds, CancellationToken cancellationToken = default);
    }
}