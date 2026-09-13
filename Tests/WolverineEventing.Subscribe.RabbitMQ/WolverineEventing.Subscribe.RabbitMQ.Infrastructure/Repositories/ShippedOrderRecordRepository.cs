using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Entities;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Repositories;
using WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ShippedOrderRecordRepository : RepositoryBase<ShippedOrderRecord, ShippedOrderRecord, ApplicationDbContext>, IShippedOrderRecordRepository
    {
        public ShippedOrderRecordRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<ShippedOrderRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ShippedOrderRecord?> FindByIdAsync(
            Guid id,
            Func<IQueryable<ShippedOrderRecord>, IQueryable<ShippedOrderRecord>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<ShippedOrderRecord>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}