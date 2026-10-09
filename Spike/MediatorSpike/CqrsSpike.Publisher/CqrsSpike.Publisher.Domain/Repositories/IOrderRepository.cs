using CqrsSpike.Publisher.Domain.Entities;

namespace CqrsSpike.Publisher.Domain.Repositories
{
    public interface IOrderRepository : IEFRepository<Order, Order>
    {
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        Task<Order?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Order?> FindByIdAsync(Guid id, Func<IQueryable<Order>, IQueryable<Order>> queryOptions, CancellationToken cancellationToken = default);
        Task<List<Order>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}