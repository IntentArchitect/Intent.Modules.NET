using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities.Accounts;
using SqlDbProject.Domain.Repositories;
using SqlDbProject.Domain.Repositories.Accounts;
using SqlDbProject.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AccountRepository : RepositoryBase<Account, Account, ApplicationDbContext>, Domain.Repositories.Accounts.IAccountRepository
    {
        public AccountRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            long accountId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.AccountId == accountId, cancellationToken);
        }

        public async Task<Account?> FindByIdAsync(long accountId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.AccountId == accountId, cancellationToken);
        }

        public async Task<Account?> FindByIdAsync(
            long accountId,
            Func<IQueryable<Account>, IQueryable<Account>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.AccountId == accountId, queryOptions, cancellationToken);
        }

        public async Task<List<Account>> FindByIdsAsync(long[] accountIds, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = accountIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.AccountId), cancellationToken);
        }
    }
}