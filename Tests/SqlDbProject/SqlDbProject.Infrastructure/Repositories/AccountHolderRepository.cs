using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities;
using SqlDbProject.Domain.Repositories;
using SqlDbProject.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AccountHolderRepository : RepositoryBase<AccountHolder, AccountHolder, ApplicationDbContext>, IAccountHolderRepository
    {
        public AccountHolderRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            long accountHolderId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.AccountHolderId == accountHolderId, cancellationToken);
        }

        public async Task<AccountHolder?> FindByIdAsync(
            long accountHolderId,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.AccountHolderId == accountHolderId, cancellationToken);
        }

        public async Task<AccountHolder?> FindByIdAsync(
            long accountHolderId,
            Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.AccountHolderId == accountHolderId, queryOptions, cancellationToken);
        }

        public async Task<List<AccountHolder>> FindByIdsAsync(
            long[] accountHolderIds,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = accountHolderIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.AccountHolderId), cancellationToken);
        }
    }
}