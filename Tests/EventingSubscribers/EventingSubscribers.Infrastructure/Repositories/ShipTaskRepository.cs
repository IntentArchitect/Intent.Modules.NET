using EventingSubscribers.Domain.Entities;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EventingSubscribers.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ShipTaskRepository : RepositoryBase<ShipTask, ShipTask, ApplicationDbContext>, IShipTaskRepository
    {
        public ShipTaskRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ShipTask?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ShipTask?> FindByIdAsync(
            Guid id,
            Func<IQueryable<ShipTask>, IQueryable<ShipTask>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<ShipTask>> FindByIdsAsync(Guid[] ids,
CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}