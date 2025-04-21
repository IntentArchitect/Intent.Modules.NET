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
    public class PeriodRepository : RepositoryBase<Period, Period, ApplicationDbContext>, IPeriodRepository
    {
        public PeriodRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int periodId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.PeriodId == periodId, cancellationToken);
        }

        public async Task<Period?> FindByIdAsync(int periodId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PeriodId == periodId, cancellationToken);
        }

        public async Task<List<Period>> FindByIdsAsync(int[] periodIds, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = periodIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.PeriodId), cancellationToken);
        }
    }
}