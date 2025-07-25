using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using SqlDbProject.Domain.Contracts;
using SqlDbProject.Domain.Repositories;
using SqlDbProject.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AccountHolderPerson> GetAccountHolderPerson(
            long stakeholderId,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.AccountHolderPeople
                .FromSqlInterpolated($"EXECUTE GetAccountHolderPerson {stakeholderId}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }
    }
}