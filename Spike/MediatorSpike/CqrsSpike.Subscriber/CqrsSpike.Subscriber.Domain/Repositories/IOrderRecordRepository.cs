using CqrsSpike.Subscriber.Domain.Entities;

namespace CqrsSpike.Subscriber.Domain.Repositories
{
    public interface IOrderRecordRepository : IEFRepository<OrderRecord, OrderRecord>
    {
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        Task<OrderRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<OrderRecord?> FindByIdAsync(Guid id, Func<IQueryable<OrderRecord>, IQueryable<OrderRecord>> queryOptions, CancellationToken cancellationToken = default);
        Task<List<OrderRecord>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}