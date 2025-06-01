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
    public class AccountTypeRepository : RepositoryBase<AccountType, AccountType, ApplicationDbContext>, IAccountTypeRepository
    {
        public AccountTypeRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int accountTypeId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.AccountTypeId == accountTypeId, cancellationToken);
        }

        public async Task<AccountType?> FindByIdAsync(int accountTypeId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.AccountTypeId == accountTypeId, cancellationToken);
        }

        public async Task<AccountType?> FindByIdAsync(
            int accountTypeId,
            Func<IQueryable<AccountType>, IQueryable<AccountType>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.AccountTypeId == accountTypeId, queryOptions, cancellationToken);
        }

        public async Task<List<AccountType>> FindByIdsAsync(
            int[] accountTypeIds,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = accountTypeIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.AccountTypeId), cancellationToken);
        }
    }
}