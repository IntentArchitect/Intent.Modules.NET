using AutoMapper;
using CqrsSpike.Subscriber.Domain.Entities;
using CqrsSpike.Subscriber.Domain.Repositories;
using CqrsSpike.Subscriber.Infrastructure.Persistence;

namespace CqrsSpike.Subscriber.Infrastructure.Repositories
{
    public class OrderRecordRepository : RepositoryBase<OrderRecord, OrderRecord, ApplicationDbContext>, IOrderRecordRepository
    {
        public OrderRecordRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<OrderRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<OrderRecord?> FindByIdAsync(
            Guid id,
            Func<IQueryable<OrderRecord>, IQueryable<OrderRecord>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<OrderRecord>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}