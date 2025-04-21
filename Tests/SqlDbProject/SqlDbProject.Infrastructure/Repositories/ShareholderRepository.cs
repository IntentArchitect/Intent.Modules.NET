using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using SqlDbProject.Domain.Contracts;
using SqlDbProject.Domain.Repositories;
using SqlDbProject.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Repositories
{
    public class ShareholderRepository : IShareholderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ShareholderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShareholderPerson> GetStakeholderPerson(
            long stakeholderId,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.ShareholderPeople
                .FromSqlInterpolated($"EXECUTE GetStakeholderPerson {stakeholderId}")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken))
                .Single();

            return result;
        }
    }
}