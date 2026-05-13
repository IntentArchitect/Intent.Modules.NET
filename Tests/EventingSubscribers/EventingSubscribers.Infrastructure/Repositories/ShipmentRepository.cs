using EventingSubscribers.Domain.Entities;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EventingSubscribers.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ShipmentRepository : RepositoryBase<Shipment, Shipment, ApplicationDbContext>, IShipmentRepository
    {
        public ShipmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Shipment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Shipment?> FindByIdAsync(
            Guid id,
            Func<IQueryable<Shipment>, IQueryable<Shipment>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<Shipment>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}