using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TestMediator.NoEventing.Domain.Products;
using TestMediator.NoEventing.Domain.Repositories;
using TestMediator.NoEventing.Domain.Repositories.Products;
using TestMediator.NoEventing.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace TestMediator.NoEventing.Infrastructure.Repositories.Products
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PriceHistoryEntryRepository : RepositoryBase<PriceHistoryEntry, PriceHistoryEntry, ApplicationDbContext>, IPriceHistoryEntryRepository
    {
        public PriceHistoryEntryRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<PriceHistoryEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<PriceHistoryEntry?> FindByIdAsync(
            Guid id,
            Func<IQueryable<PriceHistoryEntry>, IQueryable<PriceHistoryEntry>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<PriceHistoryEntry>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}